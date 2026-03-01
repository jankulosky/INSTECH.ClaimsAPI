namespace Claims.Repositories.Interfaces;

public interface IAuditRepository
{
    Task SaveClaimAuditAsync(string claimId, string httpRequestType, CancellationToken cancellationToken);
    Task SaveCoverAuditAsync(string coverId, string httpRequestType, CancellationToken cancellationToken);
}
