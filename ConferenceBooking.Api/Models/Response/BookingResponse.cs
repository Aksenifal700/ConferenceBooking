namespace ConferenceBooking.Models.Response;

public class BookingResponse
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }

    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }

    public decimal TotalPrice { get; set; }

}