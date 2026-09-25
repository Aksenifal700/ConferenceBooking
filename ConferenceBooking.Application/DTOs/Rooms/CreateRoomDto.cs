namespace ConferenceBooking.Application.DTOs.Rooms;

public class CreateRoomDto
{
    public string? Name { get; set; }
    public int Capacity { get; set; }
    public decimal HourlyRate { get; set; }

    public List<CreateRoomServiceDto> Services { get; set; } = [];
}