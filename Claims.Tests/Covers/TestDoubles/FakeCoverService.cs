using Claims.Contracts;
using Claims.Enums;
using Claims.Exceptions;
using Claims.Services.Interfaces;

namespace Claims.Tests;

internal sealed class FakeCoverService : ICoverService
{
    private readonly IReadOnlyCollection<CoverResponse> _covers;

    public FakeCoverService(IReadOnlyCollection<CoverResponse> covers)
    {
        _covers = covers;
    }

    public Task<IReadOnlyCollection<CoverResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_covers);
    }

    public Task<CoverResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var cover = _covers.SingleOrDefault(x => x.Id == id);
        if (cover is null)
        {
            throw new NotFoundException($"Cover '{id}' was not found.");
        }

        return Task.FromResult(cover);
    }

    public Task<CoverResponse> CreateAsync(CreateCoverRequest request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Create is not needed in these controller tests.");
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Delete is not needed in these controller tests.");
    }

    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return 12345m;
    }
}
