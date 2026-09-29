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

        if (endsAt <= startsAt)
        {
            throw new ArgumentException(
                "Booking end must be after its start.");
        }

        if (hourlyRate <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hourlyRate));
        }

        var openingTime = new TimeSpan(6, 0, 0);
        var closingTime = new TimeSpan(23, 0, 0);

        if (startsAt.Date != endsAt.Date ||
            startsAt.TimeOfDay < openingTime ||
            endsAt.TimeOfDay > closingTime)
        {
            throw new ArgumentException(
                "Booking must be within 06:00–23:00 UTC on the same day.");
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