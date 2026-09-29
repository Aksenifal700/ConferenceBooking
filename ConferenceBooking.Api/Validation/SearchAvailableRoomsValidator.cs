using ConferenceBooking.Models.Requests.Rooms;
using FluentValidation;

namespace ConferenceBooking.Validation;

public class SearchAvailableRoomsValidator : AbstractValidator<SearchAvailableRoomsRequest>
{
    public SearchAvailableRoomsValidator()
    {
        RuleFor(request => request.Capacity)
            .GreaterThan(0)
            .WithMessage("Capacity must be greater than zero.");

        RuleFor(request => request.StartsAt)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Start time is required.")
            .Must(startsAt => startsAt > DateTimeOffset.UtcNow)
            .WithMessage("Start time must be in the future.");

        RuleFor(request => request.EndsAt)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("End time is required.")
            .GreaterThan(request => request.StartsAt)
            .WithMessage("End time must be after start time.")
            .Must((request, endsAt) =>
                IsWithinOpeningHours(request.StartsAt, endsAt))
            .WithMessage(
                "The interval must be within 06:00–23:00 UTC on the same day.");
    }

    private static bool IsWithinOpeningHours(
        DateTimeOffset startsAt,
        DateTimeOffset endsAt)
    {
        var startUtc = startsAt.ToUniversalTime();
        var endUtc = endsAt.ToUniversalTime();

        return startUtc.Date == endUtc.Date
               && startUtc.TimeOfDay >= TimeSpan.FromHours(6)
               && endUtc.TimeOfDay <= TimeSpan.FromHours(23);
    }
}