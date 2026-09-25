namespace ConferenceBooking.Application.Interfaces.Repositories;

public interface IAdditionalServiceRepository
{
    Task<IReadOnlyList<Guid>> GetExistingIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default);
}