using Claims.Contracts;

namespace Claims.Services.Interfaces;

public interface IClaimService
{
    Task<IReadOnlyCollection<ClaimResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<ClaimResponse> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ClaimResponse> CreateAsync(CreateClaimRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
