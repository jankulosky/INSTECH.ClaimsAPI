using System.Threading.Channels;

namespace Claims.Auditing;

public sealed class AuditQueue : IAuditQueue
{
    private readonly Channel<AuditWorkItem> _channel;

    public AuditQueue()
    {
        _channel = Channel.CreateUnbounded<AuditWorkItem>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });
    }

    public bool Enqueue(AuditWorkItem workItem)
    {
        return _channel.Writer.TryWrite(workItem);
    }

    public IAsyncEnumerable<AuditWorkItem> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
