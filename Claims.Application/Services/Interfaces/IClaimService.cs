using Claims.Application.Contracts;
using Claims.Application.Models;

namespace Claims.Application.Services.Interfaces;

public interface IClaimService
{
    Task<IReadOnlyCollection<ClaimModel>> GetAllAsync(CancellationToken cancellationToken);
    Task<ClaimModel> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ClaimModel> CreateAsync(CreateClaimRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}

