using ConferenceBooking.Application.DTOs.Rooms;
using ConferenceBooking.Application.Interfaces.Services;
using ConferenceBooking.Mapping;
using ConferenceBooking.Models.Requests.Rooms;
using ConferenceBooking.Models.Response;
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
    public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] UpdateRoomRequest request,
        CancellationToken cancellationToken)
    {
        var dto = request.ToDto();
        
        await _roomService.UpdateAsync(id, dto, cancellationToken);
        
        return NoContent();
    }
}