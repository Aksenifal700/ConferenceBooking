using ConferenceBooking.Models.Requests.Rooms;
using FluentValidation;

namespace ConferenceBooking.Validation;

public class RoomServicesValidator : AbstractValidator<List<CreateRoomServiceRequest>>
{
    public RoomServicesValidator()
    {
        RuleForEach(services => services)
            .NotNull()
            .WithMessage("Service must not be null.")
            .SetValidator(new CreateRoomServiceValidator());

        RuleFor(services => services)
            .Must(HaveUniqueServiceIds)
            .WithMessage("The same service cannot be added twice.");
    }

    private static bool HaveUniqueServiceIds(List<CreateRoomServiceRequest> services)
    {
        var ids = services
            .Where(service => service is not null)
            .Select(service => service.AdditionalServiceId)
            .ToArray();

        return ids.Distinct().Count() == ids.Length;
    }
}
