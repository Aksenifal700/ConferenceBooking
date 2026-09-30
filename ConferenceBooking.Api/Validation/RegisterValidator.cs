using ConferenceBooking.Models.Requests.Auth;
using FluentValidation;

namespace ConferenceBooking.Validation;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(50);

        RuleFor(request => request.Password)
            .NotEmpty()
            .MaximumLength(30);
    }
}