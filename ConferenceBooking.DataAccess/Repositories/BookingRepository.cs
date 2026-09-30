using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.DataAccess.Database;
using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ConferenceBooking.DataAccess.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasOverlapAsync(Guid roomId, DateTimeOffset startsAt, DateTimeOffset endsAt,
        CancellationToken cancellationToken)
    {
        var startsAtUtc = startsAt.ToUniversalTime();
        var endsAtUtc = endsAt.ToUniversalTime();

        return await _context.Bookings.AnyAsync(
            booking => booking.RoomId == roomId
                       && booking.StartsAt < endsAtUtc
                       && booking.EndsAt > startsAtUtc,
            cancellationToken);
    }

    public async Task<AddBookingResult> AddAsync(
        Booking booking,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        // Serialize booking creation with archiving for this room.
        var room = await _context.Rooms
            .FromSqlInterpolated(
                $"SELECT * FROM \"Rooms\" WHERE \"Id\" = {booking.RoomId} FOR UPDATE")
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (room is null || room.IsArchived)
        {
            return AddBookingResult.RoomUnavailable;
        }

        _context.Bookings.Add(booking);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return AddBookingResult.Created;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException postgresException
                  && postgresException.SqlState == PostgresErrorCodes.ExclusionViolation
                  && postgresException.ConstraintName == "EX_Bookings_RoomId_TimeRange")
        {
            return AddBookingResult.Overlap;
        }
    }
}