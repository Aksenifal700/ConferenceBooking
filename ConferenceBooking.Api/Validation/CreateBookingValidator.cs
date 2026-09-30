using ConferenceBooking.Domain.Pricing;
using ConferenceBooking.Models.Requests.Booking;
using FluentValidation;

namespace ConferenceBooking.Validation;

public class CreateBookingValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingValidator()
    {
        RuleFor(request => request.RoomId)
            .NotEmpty()
            .WithMessage("Room ID is required.");

        RuleFor(request => request.StartsAt)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Booking start is required.")
            .Must(startsAt => startsAt > DateTimeOffset.UtcNow)
            .WithMessage("Booking must start in the future.");

        RuleFor(request => request.EndsAt)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Booking end is required.")
            .GreaterThan(request => request.StartsAt)
            .WithMessage("Booking end must be after its start.")
            .Must((request, endsAt) =>
                BookingTimeRules.IsWithinOpeningHours(request.StartsAt, endsAt))
            .WithMessage(
                "Booking must be within 06:00–23:00 UTC on the same day.");

        RuleFor(request => request.ServiceIds)
            .NotNull()
            .WithMessage("Service IDs must not be null.");

        When(request => request.ServiceIds is not null, () =>
        {
            RuleForEach(request => request.ServiceIds)
                .NotEmpty()
                .WithMessage("Service ID must not be empty.");

            RuleFor(request => request.ServiceIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("The same service cannot be selected twice.");
        });
    }
}
