namespace ConferenceBooking.Models.Requests.Rooms;

public class CreateRoomServiceRequest
{
    public Guid AdditionalServiceId { get; set; }
    public decimal Price { get; set; }
}