using Claims.Auditing;
using Claims.Repositories.Interfaces;

namespace Claims.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly IAuditQueue _auditQueue;
    private readonly ILogger<AuditRepository> _logger;

    public AuditRepository(IAuditQueue auditQueue, ILogger<AuditRepository> logger)
    {
        _auditQueue = auditQueue;
        _logger = logger;
    }

    public Task SaveClaimAuditAsync(string claimId, string httpRequestType, CancellationToken cancellationToken)
    {
        var queued = _auditQueue.Enqueue(new AuditWorkItem
        {
            EntityId = claimId,
            Created = DateTime.UtcNow,
            HttpRequestType = httpRequestType,
            Target = AuditTarget.Claim
        });

        if (!queued)
        {
            _logger.LogWarning("Claim audit could not be queued for claim id {ClaimId}.", claimId);
        }

        return Task.CompletedTask;
    }

    public Task SaveCoverAuditAsync(string coverId, string httpRequestType, CancellationToken cancellationToken)
    {
        var queued = _auditQueue.Enqueue(new AuditWorkItem
        {
            EntityId = coverId,
            Created = DateTime.UtcNow,
            HttpRequestType = httpRequestType,
            Target = AuditTarget.Cover
        });

        if (!queued)
        {
            _logger.LogWarning("Cover audit could not be queued for cover id {CoverId}.", coverId);
        }

        return Task.CompletedTask;
    }
}
