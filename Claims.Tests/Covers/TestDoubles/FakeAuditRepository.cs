using Claims.Application.Repositories.Interfaces;

namespace Claims.Tests;

internal sealed class FakeAuditRepository : IAuditRepository
{
    public List<(string ClaimId, string Verb)> ClaimAudits { get; } = [];
    public List<(string CoverId, string Verb)> CoverAudits { get; } = [];

    public void SaveClaimAudit(string claimId, string httpRequestType)
    {
        ClaimAudits.Add((claimId, httpRequestType));
    }

    public void SaveCoverAudit(string coverId, string httpRequestType)
    {
        CoverAudits.Add((coverId, httpRequestType));
    }
}

