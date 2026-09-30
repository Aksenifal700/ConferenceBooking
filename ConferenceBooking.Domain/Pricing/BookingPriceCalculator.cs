using ConferenceBooking.Domain.Entities;

namespace ConferenceBooking.Domain.Pricing;

public class BookingPriceCalculator
{
    public List<BookingPriceSegment> CalculateSegments(
        DateTimeOffset startsAt,
        DateTimeOffset endsAt,
        decimal hourlyRate)
    {
        startsAt = startsAt.ToUniversalTime();
        endsAt = endsAt.ToUniversalTime();

        // Keep direct calls safe: an invalid interval can prevent the loop from advancing.
        if (!BookingTimeRules.IsWithinOpeningHours(startsAt, endsAt))
        {
            throw new ArgumentException(
                "Booking must have a positive duration within 06:00–23:00 UTC on the same day.");
        }

        if (hourlyRate <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hourlyRate));
        }

        var segments = new List<BookingPriceSegment>();
        var currentStart = startsAt;

        while (currentStart < endsAt)
        {
            var (boundaryHour, multiplier) =
                GetTariff(currentStart.Hour);

            var tariffEnd = new DateTimeOffset(
                currentStart.Year,
                currentStart.Month,
                currentStart.Day,
                boundaryHour,
                0,
                0,
                TimeSpan.Zero);

            var segmentEnd = endsAt < tariffEnd
                ? endsAt
                : tariffEnd;

            var durationHours =
                (decimal)(segmentEnd - currentStart).Ticks
                / TimeSpan.TicksPerHour;

            segments.Add(new BookingPriceSegment
            {
                Id = Guid.NewGuid(),
                StartsAt = currentStart,
                EndsAt = segmentEnd,
                BaseHourlyRate = hourlyRate,
                Multiplier = multiplier,
                Amount = hourlyRate * multiplier * durationHours
            });

            currentStart = segmentEnd;
        }

        return segments;
    }

    private static (int BoundaryHour, decimal Multiplier) GetTariff(
        int hour)
    {
        return hour switch
        {
            < 9 => (9, 0.90m),
            < 12 => (12, 1.00m),
            < 14 => (14, 1.15m),
            < 18 => (18, 1.00m),
            _ => (23, 0.80m)
        };
    }
}