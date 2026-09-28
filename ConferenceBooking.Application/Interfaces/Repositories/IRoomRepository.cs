using ConferenceBooking.Application.DTOs.Rooms;
using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Application.Interfaces.Repositories;

public interface IRoomRepository
{
    Task AddAsync(Room room, CancellationToken cancellationToken = default);
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Guid id, UpdateRoomDto dto,CancellationToken cancellationToken = default);
}