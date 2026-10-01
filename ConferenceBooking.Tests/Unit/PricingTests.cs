using ConferenceBooking.Domain.Pricing;

namespace ConferenceBooking.Tests.Unit;

[Trait("Category", "Unit")]
public class PricingTests
{
    private readonly BookingPriceCalculator _priceCalculator;

    public PricingTests()
    {
        _priceCalculator = new BookingPriceCalculator();
    }

    [Theory]
    [InlineData(6, 9, 5400)]
    [InlineData(9, 12, 6000)]
    [InlineData(12, 14, 4600)]
    [InlineData(14, 18, 8000)]
    [InlineData(18, 23, 8000)]
    [InlineData(7, 10, 5600)]
    [InlineData(10, 15, 10600)]
    [InlineData(6, 23, 32000)]
    public void CalculateSegments_WhenIntervalCrossesTariffs_ReturnsExpectedAmount(
        int startHour, int endHour, decimal expectedAmount)
    {
        // Arrange
        var startsAt = CreateTime(startHour);
        var endsAt = CreateTime(endHour);

        // Act
        var result = _priceCalculator.CalculateSegments(startsAt, endsAt, 2000m);

        // Assert
        Assert.Equal(expectedAmount, result.Sum(segment => segment.Amount));
        Assert.Equal(startsAt, result.First().StartsAt);
        Assert.Equal(endsAt, result.Last().EndsAt);
        Assert.All(result, segment => Assert.True(segment.EndsAt > segment.StartsAt));

        for (var i = 1; i < result.Count; i++)
        {
            Assert.Equal(result[i - 1].EndsAt, result[i].StartsAt);
        }
    }

    [Fact]
    public void CalculateSegments_WhenPartialHoursCrossPeakBoundary_SplitsPrice()
    {
        // Arrange
        var startsAt = CreateTime(11).AddMinutes(30);
        var endsAt = CreateTime(12).AddMinutes(30);

        // Act
        var result = _priceCalculator.CalculateSegments(startsAt, endsAt, 2000m);

        // Assert
        Assert.Equal(2150m, result.Sum(segment => segment.Amount));
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void CalculateSegments_WhenInputHasOffset_UsesUtcTariff()
    {
        // Arrange
        // 14:00 at UTC+2 is 12:00 UTC, the peak tariff.
        var startsAt = new DateTimeOffset(2030, 1, 2, 14, 0, 0, TimeSpan.FromHours(2));
        var endsAt = startsAt.AddHours(1);

        // Act
        var result = _priceCalculator.CalculateSegments(startsAt, endsAt, 2000m);

        // Assert
        Assert.Equal(2300m, result.Sum(segment => segment.Amount));
    }

    [Theory]
    [InlineData(5, 7)]
    [InlineData(22, 24)]
    [InlineData(12, 12)]
    [InlineData(14, 12)]
    public void CalculateSegments_WhenIntervalIsInvalid_ThrowsArgumentException(
        int startHour, int endHour)
    {
        // Arrange
        var startsAt = CreateTime(startHour);
        var endsAt = CreateTime(endHour);

        // Act
        var exception = Record.Exception(
            () => _priceCalculator.CalculateSegments(startsAt, endsAt, 2000m));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    private static DateTimeOffset CreateTime(int hour)
    {
        return new DateTimeOffset(2030, 1, 2, 0, 0, 0, TimeSpan.Zero).AddHours(hour);
    }
}
