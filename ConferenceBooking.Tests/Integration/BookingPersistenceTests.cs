using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.DataAccess.Identity;
using ConferenceBooking.DataAccess.Repositories;
using ConferenceBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.Tests.Integration;

[Trait("Category", "Integration")]
public class BookingPersistenceTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _database;

    public BookingPersistenceTests(PostgresFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task AddAsync_WhenConcurrentBookingsOverlap_PersistsOnlyOne()
    {
        // Arrange
        var (room, user) = await SeedAsync();
        var firstGate = new RowLockGate(true);
        var secondGate = new RowLockGate(false);
        await using var firstContext = _database.CreateContext(firstGate);
        await using var secondContext = _database.CreateContext(secondGate);

        // Act
        var first = new BookingRepository(firstContext).AddAsync(NewBooking(room, user), default);
        await firstGate.Locked.Task.WaitAsync(TimeSpan.FromSeconds(15));
        var second = new BookingRepository(secondContext).AddAsync(NewBooking(room, user), default);
        try
        {
            await secondGate.Started.Task.WaitAsync(TimeSpan.FromSeconds(15));
        }
        finally
        {
            firstGate.Release.TrySetResult();
        }

        // Assert
        Assert.Equal(AddBookingResult.Created, await first);
        Assert.Equal(AddBookingResult.Overlap, await second);
        await using var check = _database.CreateContext();
        Assert.Equal(1, await check.Bookings.CountAsync(b => b.RoomId == room));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ArchiveAsync_WhenBookingRunsConcurrently_AllowsOnlyOneOperation(bool archiveFirst)
    {
        // Arrange
        var (room, user) = await SeedAsync();
        var gate = new RowLockGate(true);
        var contender = new RowLockGate(false);
        await using var firstContext = _database.CreateContext(gate);
        await using var secondContext = _database.CreateContext(contender);
        var now = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);

        Task<ArchiveRoomResult> archive;
        Task<AddBookingResult> booking;

        // Act
        if (archiveFirst)
        {
            archive = new RoomRepository(firstContext).ArchiveAsync(room, now);
            await gate.Locked.Task.WaitAsync(TimeSpan.FromSeconds(15));
            booking = new BookingRepository(secondContext).AddAsync(NewBooking(room, user), default);
        }
        else
        {
            booking = new BookingRepository(firstContext).AddAsync(NewBooking(room, user), default);
            await gate.Locked.Task.WaitAsync(TimeSpan.FromSeconds(15));
            archive = new RoomRepository(secondContext).ArchiveAsync(room, now);
        }

        try
        {
            await contender.Started.Task.WaitAsync(TimeSpan.FromSeconds(15));
        }
        finally
        {
            gate.Release.TrySetResult();
        }

        // Assert
        Assert.Equal(archiveFirst ? ArchiveRoomResult.Archived : ArchiveRoomResult.HasActiveBookings, await archive);
        Assert.Equal(archiveFirst ? AddBookingResult.RoomUnavailable : AddBookingResult.Created, await booking);
        await using var check = _database.CreateContext();
        Assert.Equal(archiveFirst, (await check.Rooms.FindAsync(room))!.IsArchived);
        Assert.Equal(archiveFirst ? 0 : 1, await check.Bookings.CountAsync(b => b.RoomId == room));
    }

    [Fact]
    public async Task AddAsync_WhenBookingsAreAdjacent_CreatesBoth()
    {
        // Arrange
        var (room, user) = await SeedAsync();
        await using var context = _database.CreateContext();
        var repository = new BookingRepository(context);
        var first = NewBooking(room, user);
        var second = NewBooking(room, user);
        second.StartsAt = first.EndsAt;
        second.EndsAt = second.StartsAt.AddHours(1);

        // Act
        var firstResult = await repository.AddAsync(first, default);
        var secondResult = await repository.AddAsync(second, default);

        // Assert
        Assert.Equal(AddBookingResult.Created, firstResult);
        Assert.Equal(AddBookingResult.Created, secondResult);
    }

    [Fact]
    public async Task ArchiveAsync_WhenBookingHasEnded_PreservesBookingHistory()
    {
        // Arrange
        var (room, user) = await SeedAsync();
        await using var context = _database.CreateContext();
        var booking = NewBooking(room, user);
        Assert.Equal(AddBookingResult.Created, await new BookingRepository(context).AddAsync(booking, default));

        // Act
        var result = await new RoomRepository(context).ArchiveAsync(room, booking.EndsAt);

        // Assert
        Assert.Equal(ArchiveRoomResult.Archived, result);
        Assert.True(await context.Bookings.AnyAsync(b => b.Id == booking.Id));
        Assert.Null(await new RoomRepository(context).GetByIdAsync(room));
    }

    [Fact]
    public async Task GetReportsAsync_WhenBookingsExist_AggregatesPeriodAndStoredPrices()
    {
        // Arrange
        var (room, user) = await SeedAsync();
        await using var context = _database.CreateContext();
        var booking = NewBooking(room, user);
        booking.TotalPrice = 2700m;
        booking.Services.Add(new ConferenceBooking.Domain.Entities.BookingService
        {
            BookingId = booking.Id,
            AdditionalServiceId = Guid.Parse("33333333-3333-4333-8333-333333333333"),
            Name = "Historical sound",
            Price = 700m
        });
        var outside = NewBooking(room, user);
        outside.StartsAt = outside.StartsAt.AddDays(1);
        outside.EndsAt = outside.EndsAt.AddDays(1);
        context.Bookings.AddRange(booking, outside);
        await context.SaveChangesAsync();

        var from = new DateTimeOffset(2030, 1, 2, 0, 0, 0, TimeSpan.Zero);
        var repository = new ReportRepository(context);

        // Act
        var revenueResult = await repository.GetRevenueAsync(from, from.AddDays(1), default);
        var usageResult = await repository.GetUsageAsync(from, from.AddDays(1), default);
        var services = await repository.GetServicesAsync(from, from.AddDays(1), default);

        // Assert
        var revenue = Assert.Single(revenueResult, r => r.RoomId == room);
        Assert.Equal(1, revenue.BookingCount);
        Assert.Equal(2700m, revenue.BookingTotal);
        var usage = Assert.Single(usageResult, r => r.RoomId == room);
        Assert.Equal(1d, usage.BookedHours);
        var sound = Assert.Single(services, s => s.Name == "Historical sound");
        Assert.Equal(700m, sound.TotalAmount);
    }

    private async Task<(Guid Room, Guid User)> SeedAsync()
    {
        await using var context = _database.CreateContext();
        var room = new Room { Id = Guid.NewGuid(), RoomName = "Integration", Capacity = 50, HourlyRate = 2000m };
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = Guid.NewGuid().ToString() };
        context.Rooms.Add(room);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return (room.Id, user.Id);
    }

    private static Booking NewBooking(Guid roomId, Guid userId)
    {
        return new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            UserId = userId,
            StartsAt = new DateTimeOffset(2030, 1, 2, 10, 0, 0, TimeSpan.Zero),
            EndsAt = new DateTimeOffset(2030, 1, 2, 11, 0, 0, TimeSpan.Zero),
            CreatedAt = DateTimeOffset.UtcNow,
            TotalPrice = 2000m
        };
    }
}