using ConferenceBooking.Application.DTOs.Rooms;

namespace ConferenceBooking.Application.Interfaces.Services;

public interface IRoomService
{
    Task<Guid> CreateRoomAsync(CreateRoomDto dto, CancellationToken cancellationToken = default);
    Task<RoomDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid id, UpdateRoomDto dto, CancellationToken cancellationToken = default);
}