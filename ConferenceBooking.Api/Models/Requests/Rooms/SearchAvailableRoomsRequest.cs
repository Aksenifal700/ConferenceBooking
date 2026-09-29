namespace ConferenceBooking.Models.Requests.Rooms;

public class SearchAvailableRoomsRequest
{
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public int Capacity { get; set; }
}