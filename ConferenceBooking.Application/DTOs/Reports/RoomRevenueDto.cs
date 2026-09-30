namespace ConferenceBooking.Application.DTOs.Reports;

public record RoomRevenueDto(Guid RoomId, string RoomName, bool IsArchived, int BookingCount, decimal BookingTotal);
