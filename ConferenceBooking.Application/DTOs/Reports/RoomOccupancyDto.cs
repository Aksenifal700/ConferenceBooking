namespace ConferenceBooking.Application.DTOs.Reports;

public record RoomOccupancyDto(Guid RoomId, string RoomName, bool IsArchived, double BookedHours, double WorkingHours, double OccupancyPercent);
