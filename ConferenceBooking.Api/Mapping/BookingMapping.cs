using ConferenceBooking.Application.DTOs.Bookings;
using ConferenceBooking.Models.Requests.Booking;
using ConferenceBooking.Models.Response;

namespace ConferenceBooking.Mapping;

public static class BookingMapping
{
    public static CreateBookingDto ToDto(
        this CreateBookingRequest request)
    {
        return new CreateBookingDto
        {
            RoomId = request.RoomId,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            ServiceIds = request.ServiceIds.ToList()
        };
    }

    public static BookingResponse ToResponse(this BookingDto dto)
    {
        return new BookingResponse
        {
            Id = dto.Id,
            RoomId = dto.RoomId,
            StartsAt = dto.StartsAt,
            EndsAt = dto.EndsAt,
            TotalPrice = dto.TotalPrice
        };
    }
}