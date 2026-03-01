using Claims.Contracts;
using Claims.Enums;

namespace Claims.Services.Interfaces;

public interface ICoverService
{
    Task<IReadOnlyCollection<CoverResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<CoverResponse> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<CoverResponse> CreateAsync(CreateCoverRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
    decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
}
