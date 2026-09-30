namespace ConferenceBooking.Application.DTOs.Reports;

public record ServicePopularityDto(Guid ServiceId, string Name, int BookingCount, decimal TotalAmount);
