namespace ConferenceBooking.Models.Requests.Rooms;

public class UpdateRoomRequest
{
    public required string RoomName { get; set; }
    public int Capacity { get; set; }
    public decimal HourlyRate { get; set; }

    public required List<CreateRoomServiceRequest> Services { get; set; }
}