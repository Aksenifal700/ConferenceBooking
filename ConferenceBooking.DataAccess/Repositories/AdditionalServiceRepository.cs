using ConferenceBooking.Application.Interfaces.Repositories;
using ConferenceBooking.DataAccess.Database;
using Microsoft.EntityFrameworkCore;

namespace ConferenceBooking.DataAccess.Repositories;

public class AdditionalServiceRepository : IAdditionalServiceRepository
{
    private readonly AppDbContext _context;

    public AdditionalServiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Guid>> GetExistingIdsAsync(IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        return await _context.AdditionalServices
            .Where(service => ids.Contains(service.Id))
            .Select(service => service.Id)
            .ToListAsync(cancellationToken);
    }
}