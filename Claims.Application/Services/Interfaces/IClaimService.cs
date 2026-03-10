using Claims.Application.Contracts;
using Claims.Application.Models;

namespace Claims.Application.Services.Interfaces;

/// <summary>
/// Application use cases for claims.
/// </summary>
public interface IClaimService
{
    /// <summary>
    /// Returns all claims.
    /// </summary>
    Task<IReadOnlyCollection<ClaimModel>> GetAllAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Returns a claim by identifier.
    /// </summary>
    Task<ClaimModel> GetByIdAsync(string id, CancellationToken cancellationToken);
    /// <summary>
    /// Creates a claim after business validation.
    /// </summary>
    Task<ClaimModel> CreateAsync(CreateClaimRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Deletes a claim by identifier.
    /// </summary>
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}

