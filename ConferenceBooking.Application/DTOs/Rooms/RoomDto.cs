using ConferenceBooking.Application.DTOs.RoomService;
using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Application.DTOs.Rooms;

public class RoomDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public decimal HourlyRate { get; set; }

    public List<RoomServiceDto> RoomServices { get; set; } = [];
}