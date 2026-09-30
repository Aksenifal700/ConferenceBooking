using ConferenceBooking.Application.DTOs.Reports;

namespace ConferenceBooking.Application.Interfaces.Repositories;

public interface IReportRepository
{
    Task<List<RoomRevenueDto>> GetRevenueAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
    Task<List<RoomUsageDto>> GetUsageAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
    Task<List<ServicePopularityDto>> GetServicesAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
}
