namespace Claims.Application.Auditing;

public sealed class AuditWorkItem
{
    public required AuditTarget Target { get; init; }
    public required string EntityId { get; init; }
    public required string HttpRequestType { get; init; }
    public DateTime Created { get; init; }
    public int RetryCount { get; init; }
}

