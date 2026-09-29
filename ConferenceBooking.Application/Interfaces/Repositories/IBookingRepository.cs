using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Application.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<bool> HasOverlapAsync(Guid roomId, DateTimeOffset startsAt, DateTimeOffset endsAt, CancellationToken cancellationToken);
    Task AddAsync(Booking booking, CancellationToken cancellationToken);
}