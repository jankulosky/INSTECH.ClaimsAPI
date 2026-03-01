using Claims.Repositories.Interfaces;

namespace Claims.Tests;

internal sealed class FakeAuditRepository : IAuditRepository
{
    public List<(string ClaimId, string Verb)> ClaimAudits { get; } = [];
    public List<(string CoverId, string Verb)> CoverAudits { get; } = [];

    public Task SaveClaimAuditAsync(string claimId, string httpRequestType, CancellationToken cancellationToken)
    {
        ClaimAudits.Add((claimId, httpRequestType));
        return Task.CompletedTask;
    }

    public Task SaveCoverAuditAsync(string coverId, string httpRequestType, CancellationToken cancellationToken)
    {
        CoverAudits.Add((coverId, httpRequestType));
        return Task.CompletedTask;
    }
}
