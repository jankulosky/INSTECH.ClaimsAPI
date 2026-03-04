using Claims.Application.Contracts;
using Claims.Application.Exceptions;
using Claims.Application.Models;
using Claims.Application.Services.Interfaces;

namespace Claims.Tests;

internal sealed class FakeClaimService : IClaimService
{
    private readonly IReadOnlyCollection<ClaimModel> _claims;

    public FakeClaimService(IReadOnlyCollection<ClaimModel> claims)
    {
        _claims = claims;
    }

    public Task<IReadOnlyCollection<ClaimModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_claims);
    }

    public Task<ClaimModel> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var claim = _claims.SingleOrDefault(x => x.Id == id);
        if (claim is null)
        {
            throw new NotFoundException($"Claim '{id}' was not found.");
        }

        return Task.FromResult(claim);
    }

    public Task<ClaimModel> CreateAsync(CreateClaimRequest request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Create is not needed in these controller tests.");
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        throw new NotSupportedException("Delete is not needed in these controller tests.");
    }
}

