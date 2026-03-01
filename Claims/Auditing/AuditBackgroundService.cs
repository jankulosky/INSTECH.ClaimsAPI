using Claims.Data;
using Claims.Entities;

namespace Claims.Auditing;

public sealed class AuditBackgroundService : BackgroundService
{
    private const int MaxRetryCount = 5;
    private readonly IAuditQueue _auditQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuditBackgroundService> _logger;

    public AuditBackgroundService(
        IAuditQueue auditQueue,
        IServiceScopeFactory scopeFactory,
        ILogger<AuditBackgroundService> logger)
    {
        _auditQueue = auditQueue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var workItem in _auditQueue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await SaveAuditAsync(workItem, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                if (workItem.RetryCount < MaxRetryCount)
                {
                    var retryDelay = TimeSpan.FromSeconds(Math.Pow(2, workItem.RetryCount));
                    await Task.Delay(retryDelay, stoppingToken);

                    var requeued = _auditQueue.Enqueue(new AuditWorkItem
                    {
                        Target = workItem.Target,
                        EntityId = workItem.EntityId,
                        HttpRequestType = workItem.HttpRequestType,
                        Created = workItem.Created,
                        RetryCount = workItem.RetryCount + 1
                    });

                    if (!requeued)
                    {
                        _logger.LogError(
                            ex,
                            "Failed to requeue audit item for {Target} with id {EntityId}.",
                            workItem.Target,
                            workItem.EntityId);
                    }

                    continue;
                }

                _logger.LogError(
                    ex,
                    "Dropping audit item after {RetryCount} retries for {Target} with id {EntityId}.",
                    workItem.RetryCount,
                    workItem.Target,
                    workItem.EntityId);
            }
        }
    }

    private async Task SaveAuditAsync(AuditWorkItem workItem, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ClaimsDbContext>();

        switch (workItem.Target)
        {
            case AuditTarget.Claim:
                dbContext.ClaimAudits.Add(new ClaimAudit
                {
                    ClaimId = workItem.EntityId,
                    Created = workItem.Created,
                    HttpRequestType = workItem.HttpRequestType
                });
                break;
            case AuditTarget.Cover:
                dbContext.CoverAudits.Add(new CoverAudit
                {
                    CoverId = workItem.EntityId,
                    Created = workItem.Created,
                    HttpRequestType = workItem.HttpRequestType
                });
                break;
            default:
                throw new InvalidOperationException($"Unsupported audit target '{workItem.Target}'.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
