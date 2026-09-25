using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Application.Interfaces.Repositories;

public interface IRoomRepository
{
    Task AddAsync(Room room, CancellationToken cancellationToken = default);
}