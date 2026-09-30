namespace ConferenceBooking.Domain.Entities;

public class Room
{
    public Guid Id { get; set; }
    public required string RoomName { get; set; }
    public int Capacity { get; set; }
    public bool IsArchived { get; set; }
    public decimal HourlyRate { get; set; }

    public ICollection<RoomService> Services { get; set; } = new List<RoomService>();
}