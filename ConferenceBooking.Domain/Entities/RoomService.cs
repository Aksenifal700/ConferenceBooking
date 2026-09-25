namespace ConferenceBooking.Domain.Entities;

public class RoomService
{
    public decimal Price { get; set; }

    public Guid RoomId { get; set; }
    public Guid AdditionalServiceId { get; set; }
}