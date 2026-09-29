namespace ConferenceBooking.Domain.Entities;

public class BookingPriceSegment
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }

    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }

    public decimal BaseHourlyRate { get; set; }
    public decimal Multiplier { get; set; }
    public decimal Amount { get; set; }
}