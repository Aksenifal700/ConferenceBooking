using ConferenceBooking.Application.DTOs.Bookings;
using ConferenceBooking.Application.Exceptions;
using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.Application.Interfaces.Services;
using ConferenceBooking.Application.Mapping;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Pricing;
using BookingServiceEntity = ConferenceBooking.Domain.Entities.BookingService;

namespace ConferenceBooking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly BookingPriceCalculator _priceCalculator;

    public BookingService(IRoomRepository roomRepository, IBookingRepository bookingRepository,
        BookingPriceCalculator priceCalculator)
    {
        _roomRepository = roomRepository;
        _bookingRepository = bookingRepository;
        _priceCalculator = priceCalculator;
    }

    public async Task<BookingDto> CreateAsync(CreateBookingDto dto, Guid userId,
        CancellationToken cancellationToken)
    {
        var startsAt = dto.StartsAt.ToUniversalTime();
        var endsAt = dto.EndsAt.ToUniversalTime();

        var room = await _roomRepository.GetByIdAsync(dto.RoomId, cancellationToken);

        if (room is null)
        {
            throw new NotFoundException("Room was not found.");
        }

        var hasOverlap = await _bookingRepository.HasOverlapAsync(
            room.Id,
            startsAt,
            endsAt,
            cancellationToken);

        if (hasOverlap)
        {
            throw new ConflictException("The room is already booked for selected time.");
        }

        var bookingId = Guid.NewGuid();

        var selectedServices = CreateBookingServices(
            bookingId,
            room,
            dto.ServiceIds);

        var segments = _priceCalculator.CalculateSegments(
            startsAt,
            endsAt,
            room.HourlyRate);

        foreach (var segment in segments)
        {
            segment.BookingId = bookingId;
        }

        var totalPrice = decimal.Round(
            segments.Sum(segment => segment.Amount)
            + selectedServices.Sum(service => service.Price),
            2,
            MidpointRounding.AwayFromZero);

        var booking = new Booking
        {
            Id = bookingId,
            RoomId = room.Id,
            UserId = userId,
            StartsAt = startsAt,
            EndsAt = endsAt,
            CreatedAt = DateTimeOffset.UtcNow,
            TotalPrice = totalPrice,
            Services = selectedServices,
            PriceSegments = segments
        };

        await _bookingRepository.AddAsync(
            booking,
            cancellationToken);

        return booking.ToDto();
    }

    private static List<BookingServiceEntity> CreateBookingServices(
        Guid bookingId,
        Room room,
        IReadOnlyCollection<Guid> serviceIds)
    {
        var roomServices = room.Services.ToDictionary(
            service => service.AdditionalServiceId);

        var result = new List<BookingServiceEntity>();

        foreach (var serviceId in serviceIds)
        {
            if (!roomServices.TryGetValue(serviceId, out var roomService))
            {
                throw new ConflictException(
                    "A selected service is not available for this room.");
            }

            result.Add(new BookingServiceEntity
            {
                BookingId = bookingId,
                AdditionalServiceId = serviceId,
                Name = roomService.AdditionalService.Name,
                Price = roomService.Price
            });
        }

        return result;
    }
}

