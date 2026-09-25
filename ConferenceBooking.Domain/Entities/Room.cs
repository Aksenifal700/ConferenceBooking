namespace ConferenceBooking.Domain.Entities;

public class Room
{
    public Guid Id { get; set; }
    public string RoomName { get; set; }
    public int Capacity { get; set; }
    public bool IsArchived { get; set; }
    public decimal   HourlyRate { get; set; }
}