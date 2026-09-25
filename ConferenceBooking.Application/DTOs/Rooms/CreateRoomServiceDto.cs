namespace ConferenceBooking.Application.DTOs.Rooms;

public class CreateRoomServiceDto
{
    public Guid AdditionalServiceId { get; set; }
    public decimal Price { get; set; }
}