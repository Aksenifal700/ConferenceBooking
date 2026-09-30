namespace ConferenceBooking.Application.DTOs.Reports;

public record RoomUsageDto(Guid RoomId, string RoomName, bool IsArchived, double BookedHours);
