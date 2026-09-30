using ConferenceBooking.Application.Interfaces.Services;
using ConferenceBooking.Mapping;
using ConferenceBooking.Models.Requests.Rooms;
using ConferenceBooking.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceBooking.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var dto = request.ToDto();

        var roomId = await _roomService.CreateRoomAsync(
            dto,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            new { id = roomId });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomResponse>> GetRoomById(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _roomService.GetByIdAsync(id, cancellationToken);

        return Ok(dto.ToResponse());
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var dto = request.ToDto();

        await _roomService.UpdateAsync(id, dto, cancellationToken);

        return NoContent();
    }

    [HttpGet("available")]
    public async Task<ActionResult<List<RoomResponse>>> GetAvailableRooms([FromQuery] SearchAvailableRoomsRequest request,
        CancellationToken cancellationToken)
    {
        var rooms = await _roomService.GetAvailableAsync(
            request.StartsAt,
            request.EndsAt,
            request.Capacity,
            cancellationToken);

        var response = rooms
            .Select(room => room.ToResponse())
            .ToList();

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteRoom(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _roomService.ArchiveAsync(id, cancellationToken);

        return NoContent();
    }
}