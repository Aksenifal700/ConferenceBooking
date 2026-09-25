using ConferenceBooking.Models.Requests.Rooms;
using FluentValidation;

namespace ConferenceBooking.Validation;

public class CreateRoomServiceValidator : AbstractValidator<CreateRoomServiceRequest>
{
    public CreateRoomServiceValidator()
    {
        RuleFor(x => x.AdditionalServiceId)
            .NotEmpty()
            .WithMessage("Service Id is required");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Service price must not be negative");
    }
}