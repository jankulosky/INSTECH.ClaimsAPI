namespace Claims.Application.Repositories.Interfaces;

public interface IAuditRepository
{
    void SaveClaimAudit(string claimId, string httpRequestType);
    void SaveCoverAudit(string coverId, string httpRequestType);
}

