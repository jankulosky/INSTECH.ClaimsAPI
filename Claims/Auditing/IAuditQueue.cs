namespace Claims.Auditing;

public interface IAuditQueue
{
    bool Enqueue(AuditWorkItem workItem);
    IAsyncEnumerable<AuditWorkItem> ReadAllAsync(CancellationToken cancellationToken);
}
