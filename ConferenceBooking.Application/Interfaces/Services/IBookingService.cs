using ConferenceBooking.Application.DTOs.Bookings;

namespace ConferenceBooking.Application.Interfaces.Services;

public interface IBookingService
{
    Task<BookingDto> CreateAsync(CreateBookingDto dto, Guid userId, CancellationToken cancellationToken);
}