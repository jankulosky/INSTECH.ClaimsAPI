using Claims.Application.Contracts;
using Claims.Application.Models;
using Claims.Enums;

namespace Claims.Application.Services.Interfaces;

public interface ICoverService
{
    Task<IReadOnlyCollection<CoverModel>> GetAllAsync(CancellationToken cancellationToken);
    Task<CoverModel> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<CoverModel> CreateAsync(CreateCoverRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
    decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
}

