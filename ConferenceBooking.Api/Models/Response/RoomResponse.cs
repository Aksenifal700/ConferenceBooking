namespace ConferenceBooking.Models.Response;

public class RoomResponse
{
    public Guid Id { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal HourlyRate { get; set; }

    public List<RoomServiceResponse> Services { get; set; } = [];
}