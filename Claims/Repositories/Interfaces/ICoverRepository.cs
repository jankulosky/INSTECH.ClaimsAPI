using Claims.Entities;

namespace Claims.Repositories.Interfaces;

public interface ICoverRepository
{
    Task<IReadOnlyCollection<Cover>> GetAllAsync(CancellationToken cancellationToken);
    Task<Cover?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task CreateAsync(Cover cover, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}
