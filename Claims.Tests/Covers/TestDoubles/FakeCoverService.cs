using Claims.Application.Contracts;
using Claims.Application.Models;
using Claims.Enums;
using Claims.Application.Exceptions;
using Claims.Application.Services.Interfaces;

namespace Claims.Tests;

internal sealed class FakeCoverService : ICoverService
{
    private readonly IReadOnlyCollection<CoverModel> _covers;

    public FakeCoverService(IReadOnlyCollection<CoverModel> covers)
    {
        _covers = covers;
    }

    public Task<IReadOnlyCollection<CoverModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_covers);
    }

    public Task<CoverModel> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var cover = _covers.SingleOrDefault(x => x.Id == id);
        if (cover is null)
        {
            throw new NotFoundException($"Cover '{id}' was not found.");
        }

        return Task.FromResult(cover);
    }

    public Task<CoverModel> CreateAsync(CreateCoverRequest request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Create is not needed in these controller tests.");
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Delete is not needed in these controller tests.");
    }

    public Task<decimal> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType, CancellationToken cancellationToken)
    {
        return Task.FromResult(12345m);
    }
}

