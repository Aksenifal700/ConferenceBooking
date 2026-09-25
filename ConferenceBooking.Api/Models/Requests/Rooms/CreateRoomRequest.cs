namespace ConferenceBooking.Models.Requests.Rooms;

public class CreateRoomRequest
{
    public string? RoomName { get; set; }
    public int Capacity { get; set; }
    public decimal HourlyRate { get; set; }
    public List<CreateRoomServiceRequest> Services { get; set; } = [];
}