using ConferenceBooking.Application.DTOs.Reports;
using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.DataAccess.Repositories;

public class ReportRepository(AppDbContext context) : IReportRepository
{
    public async Task<List<RoomRevenueDto>> GetRevenueAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        var bookings = context.Bookings.Where(b => b.StartsAt >= from && b.StartsAt < to);

        return await context.Rooms.AsNoTracking()
            .OrderBy(room => room.RoomName).ThenBy(room => room.Id)
            .Select(room => new RoomRevenueDto(
                room.Id, room.RoomName, room.IsArchived,
                bookings.Count(b => b.RoomId == room.Id),
                bookings.Where(b => b.RoomId == room.Id).Sum(b => (decimal?)b.TotalPrice) ?? 0m))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<RoomUsageDto>> GetUsageAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        return await context.Rooms.AsNoTracking()
            .OrderBy(room => room.RoomName).ThenBy(room => room.Id)
            .Select(room => new RoomUsageDto(
                room.Id, room.RoomName, room.IsArchived,
                context.Bookings
                    .Where(b => b.RoomId == room.Id && b.StartsAt < to && b.EndsAt > from)
                    .Sum(b => (double?)((b.EndsAt < to ? b.EndsAt : to)
                        - (b.StartsAt > from ? b.StartsAt : from)).TotalHours) ?? 0d))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ServicePopularityDto>> GetServicesAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        var query = from service in context.BookingServices.AsNoTracking()
                    join booking in context.Bookings on service.BookingId equals booking.Id
                    where booking.StartsAt >= @from && booking.StartsAt < to
                    group service by new { service.AdditionalServiceId, service.Name } into services
                    orderby services.Count() descending, services.Key.Name, services.Key.AdditionalServiceId
                    select new ServicePopularityDto(
                        services.Key.AdditionalServiceId, services.Key.Name,
                        services.Count(), services.Sum(service => service.Price));

        return await query.ToListAsync(cancellationToken);
    }
}
