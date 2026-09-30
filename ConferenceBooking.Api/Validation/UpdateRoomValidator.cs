using ConferenceBooking.Models.Requests.Rooms;
using FluentValidation;

namespace ConferenceBooking.Validation;

public class UpdateRoomValidator : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomValidator()
    {
        RuleFor(request => request.RoomName)
            .NotEmpty()
            .WithMessage("Room name is required.");

        RuleFor(request => request.Capacity)
            .GreaterThan(0)
            .WithMessage("Room capacity must be greater than zero.");

        RuleFor(request => request.HourlyRate)
            .GreaterThan(0)
            .WithMessage("Room hourly rate must be greater than zero.")
            .PrecisionScale(18, 2, true)
            .WithMessage("Price must have at most 16 integer digits and 2 decimal places.");

        RuleFor(request => request.Services)
            .NotNull()
            .WithMessage("Services must not be null.")
            .SetValidator(new RoomServicesValidator());
    }
}
