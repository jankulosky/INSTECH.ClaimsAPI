using Claims.Entities;

namespace Claims.Repositories.Interfaces;

public interface IClaimRepository
{
    Task<IReadOnlyCollection<Claim>> GetAllAsync(CancellationToken cancellationToken);
    Task<Claim?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task CreateAsync(Claim claim, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}
