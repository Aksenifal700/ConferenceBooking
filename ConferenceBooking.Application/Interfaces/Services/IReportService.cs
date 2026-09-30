using ConferenceBooking.Application.DTOs.Reports;

namespace ConferenceBooking.Application.Interfaces.Services;

public interface IReportService
{
    Task<List<RoomRevenueDto>> GetRevenueAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken);
    Task<List<RoomOccupancyDto>> GetOccupancyAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken);
    Task<List<ServicePopularityDto>> GetServicesAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken);
}
