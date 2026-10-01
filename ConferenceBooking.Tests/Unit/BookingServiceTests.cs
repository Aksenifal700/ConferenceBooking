using Moq;
using ConferenceBooking.Application.DTOs.Bookings;
using ConferenceBooking.Application.Exceptions;
using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.Domain.Entities;
using ConferenceBooking.Domain.Pricing;
using BookingService = ConferenceBooking.Application.Services.BookingService;
using RoomService = ConferenceBooking.Domain.Entities.RoomService;

namespace ConferenceBooking.Tests.Unit;

[Trait("Category", "Unit")]
public class BookingServiceTests
{
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _bookingService = new BookingService(
            _roomRepositoryMock.Object,
            _bookingRepositoryMock.Object,
            new BookingPriceCalculator());
    }

    [Fact]
    public async Task CreateAsync_WhenServiceSelected_ChargesOnceAndPreservesSnapshot()
    {
        // Arrange
        var room = CreateRoom();
        var dto = CreateRequest(room.Id);
        var projector = new AdditionalService
        {
            Id = Guid.NewGuid(),
            Name = "Projector"
        };
        var roomService = new RoomService
        {
            RoomId = room.Id,
            AdditionalServiceId = projector.Id,
            Price = 500m,
            AdditionalService = projector
        };
        room.Services.Add(roomService);
        dto.ServiceIds.Add(projector.Id);
        Booking? savedBooking = null;

        _roomRepositoryMock
            .Setup(x => x.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        _bookingRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Callback<Booking, CancellationToken>((booking, _) => savedBooking = booking)
            .ReturnsAsync(AddBookingResult.Created);

        // Act
        var result = await _bookingService.CreateAsync(dto, Guid.NewGuid(), default);
        roomService.Price = 999m;
        projector.Name = "Renamed";

        // Assert
        Assert.Equal(11100m, result.TotalPrice);
        Assert.NotNull(savedBooking);
        var snapshot = Assert.Single(savedBooking.Services);
        Assert.Equal(500m, snapshot.Price);
        Assert.Equal("Projector", snapshot.Name);
        _bookingRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenTotalHasHalfCent_RoundsAwayFromZero()
    {
        // Arrange
        var room = CreateRoom();
        room.HourlyRate = 1.01m;
        var dto = CreateRequest(room.Id);
        dto.EndsAt = dto.StartsAt.AddMinutes(30);

        _roomRepositoryMock
            .Setup(x => x.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        _bookingRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AddBookingResult.Created);

        // Act
        var result = await _bookingService.CreateAsync(dto, Guid.NewGuid(), default);

        // Assert
        Assert.Equal(0.51m, result.TotalPrice);
    }

    [Fact]
    public async Task CreateAsync_WhenRoomDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var dto = CreateRequest(Guid.NewGuid());

        _roomRepositoryMock
            .Setup(x => x.GetByIdAsync(dto.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        // Act
        var exception = await Record.ExceptionAsync(
            () => _bookingService.CreateAsync(dto, Guid.NewGuid(), default));

        // Assert
        Assert.IsType<NotFoundException>(exception);
        _bookingRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenServiceIsUnavailable_ThrowsConflictException()
    {
        // Arrange
        var room = CreateRoom();
        var dto = CreateRequest(room.Id);
        dto.ServiceIds.Add(Guid.NewGuid());

        _roomRepositoryMock
            .Setup(x => x.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        // Act
        var exception = await Record.ExceptionAsync(
            () => _bookingService.CreateAsync(dto, Guid.NewGuid(), default));

        // Assert
        Assert.IsType<ConflictException>(exception);
        _bookingRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData(AddBookingResult.Overlap, typeof(ConflictException))]
    [InlineData(AddBookingResult.RoomUnavailable, typeof(NotFoundException))]
    public async Task CreateAsync_WhenConcurrentOperationPreventsSave_ThrowsBusinessException(
        AddBookingResult outcome,
        Type expectedExceptionType)
    {
        // Arrange
        var room = CreateRoom();
        var dto = CreateRequest(room.Id);

        _roomRepositoryMock
            .Setup(x => x.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        _bookingRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(outcome);

        // Act
        var exception = await Record.ExceptionAsync(
            () => _bookingService.CreateAsync(dto, Guid.NewGuid(), default));

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(expectedExceptionType, exception.GetType());
    }

    private static Room CreateRoom()
    {
        return new Room
        {
            Id = Guid.NewGuid(),
            RoomName = "A",
            Capacity = 50,
            HourlyRate = 2000m
        };
    }

    private static CreateBookingDto CreateRequest(Guid roomId)
    {
        return new CreateBookingDto
        {
            RoomId = roomId,
            StartsAt = new DateTimeOffset(2030, 1, 2, 10, 0, 0, TimeSpan.Zero),
            EndsAt = new DateTimeOffset(2030, 1, 2, 15, 0, 0, TimeSpan.Zero)
        };
    }
}
