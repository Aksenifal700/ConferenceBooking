namespace ConferenceBooking.Application.DTOs.Rooms;

public class UpdateRoomDto
{
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public decimal HourlyRate { get; set; }

    public required List<CreateRoomServiceDto> Services { get; set; }
}