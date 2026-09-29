namespace ConferenceBooking.Application.DTOs.Bookings;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }

    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }

    public decimal TotalPrice { get; set; }
}