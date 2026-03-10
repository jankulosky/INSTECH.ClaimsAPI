using Claims.Application.Contracts;
using Claims.Application.Models;
using Claims.Enums;

namespace Claims.Application.Services.Interfaces;

/// <summary>
/// Application use cases for covers.
/// </summary>
public interface ICoverService
{
    /// <summary>
    /// Returns all covers.
    /// </summary>
    Task<IReadOnlyCollection<CoverModel>> GetAllAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Returns a cover by identifier.
    /// </summary>
    Task<CoverModel> GetByIdAsync(string id, CancellationToken cancellationToken);
    /// <summary>
    /// Creates a cover after business validation.
    /// </summary>
    Task<CoverModel> CreateAsync(CreateCoverRequest request, CancellationToken cancellationToken);
    /// <summary>
    /// Deletes a cover by identifier.
    /// </summary>
    Task DeleteAsync(string id, CancellationToken cancellationToken);
    /// <summary>
    /// Computes premium for a period and cover type.
    /// </summary>
    Task<decimal> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType, CancellationToken cancellationToken);
}

