using ConferenceBooking.Application.DTOs.Rooms;
using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.DataAccess.Database;
using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.DataAccess.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;

    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        _context.Rooms.Add(room);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Rooms
            .AsNoTracking()
            .Include(room => room.Services)
            .ThenInclude(service => service.AdditionalService)
            .FirstOrDefaultAsync(room => room.Id == id && !room.IsArchived,
                cancellationToken);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var room = await _context.Rooms
            .Include(room => room.Services)
            .FirstOrDefaultAsync(
                room => room.Id == id && !room.IsArchived,
                cancellationToken);

        if (room is null)
        {
            return false;
        }

        room.RoomName = dto.Name.Trim();
        room.Capacity = dto.Capacity;
        room.HourlyRate = dto.HourlyRate;

        var requestedServices = dto.Services.ToDictionary(
            service => service.AdditionalServiceId);

        foreach (var existingService in room.Services.ToList())
        {
            if (requestedServices.TryGetValue(
                    existingService.AdditionalServiceId,
                    out var requestedService))
            {
                existingService.Price = requestedService.Price;

                requestedServices.Remove(
                    existingService.AdditionalServiceId);
            }
            else
            {
                room.Services.Remove(existingService);
                _context.RoomServices.Remove(existingService);
            }
        }

        foreach (var newService in requestedServices.Values)
        {
            room.Services.Add(new RoomService
            {
                RoomId = room.Id,
                AdditionalServiceId = newService.AdditionalServiceId,
                Price = newService.Price
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<Room>> GetAvailableAsync(DateTimeOffset startsAt, DateTimeOffset endsAt,
        int capacity, CancellationToken cancellationToken = default)
    {
        var startsAtUtc = startsAt.ToUniversalTime();
        var endsAtUtc = endsAt.ToUniversalTime();

        return await _context.Rooms
            .AsNoTracking()
            .Where(room =>
                !room.IsArchived &&
                room.Capacity >= capacity &&
                !_context.Bookings.Any(booking =>
                    booking.RoomId == room.Id &&
                    booking.StartsAt < endsAtUtc &&
                    booking.EndsAt > startsAtUtc))
            .Include(room => room.Services)
            .OrderBy(room => room.Capacity)
            .ThenBy(room => room.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<ArchiveRoomResult> ArchiveAsync(
        Guid id,
        DateTimeOffset now,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        // Use the same row lock as booking creation, keeping it through the check and update.
        // Otherwise a booking could be inserted after the check but before archiving.
        var room = await _context.Rooms
            .FromSqlInterpolated(
                $"SELECT * FROM \"Rooms\" WHERE \"Id\" = {id} FOR UPDATE")
            .SingleOrDefaultAsync(cancellationToken);

        if (room is null || room.IsArchived)
        {
            return ArchiveRoomResult.NotFound;
        }

        // EndsAt > now covers both ongoing and future bookings; completed ones stay as history.
        var hasActiveBookings = await _context.Bookings.AnyAsync(
            booking => booking.RoomId == id && booking.EndsAt > now,
            cancellationToken);

        if (hasActiveBookings)
        {
            return ArchiveRoomResult.HasActiveBookings;
        }

        room.IsArchived = true;

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return ArchiveRoomResult.Archived;
    }
}