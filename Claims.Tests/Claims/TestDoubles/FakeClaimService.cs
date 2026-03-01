using Claims.Contracts;
using Claims.Exceptions;
using Claims.Services.Interfaces;

namespace Claims.Tests;

internal sealed class FakeClaimService : IClaimService
{
    private readonly IReadOnlyCollection<ClaimResponse> _claims;

    public FakeClaimService(IReadOnlyCollection<ClaimResponse> claims)
    {
        _claims = claims;
    }

    public Task<IReadOnlyCollection<ClaimResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_claims);
    }

    public Task<ClaimResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var claim = _claims.SingleOrDefault(x => x.Id == id);
        if (claim is null)
        {
            throw new NotFoundException($"Claim '{id}' was not found.");
        }

        return Task.FromResult(claim);
    }

    public Task<ClaimResponse> CreateAsync(CreateClaimRequest request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Create is not needed in these controller tests.");
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Delete is not needed in these controller tests.");
    }
}
