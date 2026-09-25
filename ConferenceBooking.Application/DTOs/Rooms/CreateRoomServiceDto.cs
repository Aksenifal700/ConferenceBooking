namespace ConferenceBooking.Application.DTOs.Rooms;

public class CreateRoomServiceDto
{
    public Guid AdditinalServiceId { get; set; }
    public decimal Price { get; set; }
}