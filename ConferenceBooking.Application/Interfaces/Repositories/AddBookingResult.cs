namespace ConferenceBooking.Application.Interfaces.Repositories;

public enum AddBookingResult
{
    Created,
    RoomUnavailable,
    Overlap
}