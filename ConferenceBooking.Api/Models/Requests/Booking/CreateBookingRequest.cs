namespace ConferenceBooking.Models.Requests.Booking;

public class CreateBookingRequest
{
    public Guid RoomId { get; set; }

    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }

    public List<Guid> ServiceIds { get; set; } = [];
}