namespace ConferenceBooking.Application.DTOs.Bookings;

public class CreateBookingDto
{
    public Guid RoomId { get; set; }

    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }

    public List<Guid> ServiceIds { get; set; } = [];
}