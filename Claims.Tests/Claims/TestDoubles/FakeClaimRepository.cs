using Claims.Entities;
using Claims.Repositories.Interfaces;

namespace Claims.Tests;

internal sealed class FakeClaimRepository : IClaimRepository
{
    private readonly List<Claim> _claims = [];

    public Task<IReadOnlyCollection<Claim>> GetAllAsync(CancellationToken cancellationToken) =>
        Task.FromResult((IReadOnlyCollection<Claim>)_claims);

    public Task<Claim?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        Task.FromResult(_claims.SingleOrDefault(c => c.Id == id));

    public Task CreateAsync(Claim claim, CancellationToken cancellationToken)
    {
        _claims.Add(claim);
        return Task.CompletedTask;
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var claim = _claims.SingleOrDefault(c => c.Id == id);
        if (claim is null)
        {
            return Task.FromResult(false);
        }

        _claims.Remove(claim);
        return Task.FromResult(true);
    }
}
