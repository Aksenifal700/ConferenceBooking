namespace ConferenceBooking.Domain.Pricing;

public static class BookingTimeRules
{
    public static bool IsWithinOpeningHours(
        DateTimeOffset startsAt,
        DateTimeOffset endsAt)
    {
        var startUtc = startsAt.ToUniversalTime();
        var endUtc = endsAt.ToUniversalTime();

        return endUtc > startUtc
            && startUtc.Date == endUtc.Date
            && startUtc.TimeOfDay >= TimeSpan.FromHours(6)
            && endUtc.TimeOfDay <= TimeSpan.FromHours(23);
    }
}
