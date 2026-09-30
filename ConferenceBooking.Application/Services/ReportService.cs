using ConferenceBooking.Application.DTOs.Reports;
using ConferenceBooking.Application.Exceptions;
using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.Application.Interfaces.Services;

namespace ConferenceBooking.Application.Services;

public class ReportService(IReportRepository repository) : IReportService
{
    public Task<List<RoomRevenueDto>> GetRevenueAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        var period = GetPeriod(from, to);
        return repository.GetRevenueAsync(period.From, period.To, cancellationToken);
    }

    public async Task<List<RoomOccupancyDto>> GetOccupancyAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        var period = GetPeriod(from, to);
        var rooms = await repository.GetUsageAsync(period.From, period.To, cancellationToken);
        var workingHours = (to.DayNumber - from.DayNumber) * 17d;

        return rooms.Select(room => new RoomOccupancyDto(
            room.RoomId, room.RoomName, room.IsArchived,
            Math.Round(room.BookedHours, 2),
            workingHours,
            Math.Round(room.BookedHours / workingHours * 100, 2))).ToList();
    }

    public Task<List<ServicePopularityDto>> GetServicesAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        var period = GetPeriod(from, to);
        return repository.GetServicesAsync(period.From, period.To, cancellationToken);
    }

    private static (DateTimeOffset From, DateTimeOffset To) GetPeriod(DateOnly from, DateOnly to)
    {
        if (from == default || to <= from || to.DayNumber - from.DayNumber > 366)
            throw new BadRequestException("Report period must be between 1 and 366 days.");

        return (
            new DateTimeOffset(from.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero),
            new DateTimeOffset(to.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero));
    }
}
