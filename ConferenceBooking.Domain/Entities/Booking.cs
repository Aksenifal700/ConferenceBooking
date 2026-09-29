namespace ConferenceBooking.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public decimal TotalPrice { get; set; }

    public ICollection<BookingService> Services { get; set; }
        = new List<BookingService>();

    public ICollection<BookingPriceSegment> PriceSegments { get; set; }
        = new List<BookingPriceSegment>();
}