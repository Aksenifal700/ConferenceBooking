namespace ConferenceBooking.Domain.Entities;

public class BookingService
{
    public Guid BookingId { get; set; }
    public Guid AdditionalServiceId { get; set; }

    public required string Name { get; set; }
    public decimal Price { get; set; }
}