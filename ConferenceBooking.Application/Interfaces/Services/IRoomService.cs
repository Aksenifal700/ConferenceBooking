using ConferenceBooking.Application.DTOs.Rooms;

namespace ConferenceBooking.Application.Interfaces.Services;

public interface IRoomService
{
    Task<Guid> CreateRoomAsync(CreateRoomDto dto, CancellationToken cancellationToken = default);
}