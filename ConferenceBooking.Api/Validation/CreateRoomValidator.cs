using ConferenceBooking.Models.Requests.Rooms;
using FluentValidation;

namespace ConferenceBooking.Validation;

public class CreateRoomValidator : AbstractValidator<CreateRoomRequest>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.RoomName)
            .NotEmpty()
            .WithMessage("Room name is required");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Room capacity must be greater than 0");

        RuleFor(x => x.HourlyRate)
            .GreaterThan(0)
            .WithMessage("Room hourly rate must be greater than 0")
            .PrecisionScale(18, 2, true)
            .WithMessage("Price must have at most 16 integer digits and 2 decimal places.");

        RuleFor(request => request.Services)
            .NotNull()
            .WithMessage("Services must not be null.")
            .SetValidator(new RoomServicesValidator());
    }
}
