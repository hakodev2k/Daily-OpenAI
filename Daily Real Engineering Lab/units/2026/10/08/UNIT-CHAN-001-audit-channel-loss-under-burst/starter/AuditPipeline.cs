using System.Threading.Channels;

public sealed record AuditEvent(int Sequence, string Action);

public sealed class AuditPipeline
{
    private readonly Channel<AuditEvent> _events = Channel.CreateBounded<AuditEvent>(
        new BoundedChannelOptions(4)
        {
            SingleReader = true,
            SingleWriter = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

    // API contract: an accepted event must be observed by the active consumer
    // while the process remains healthy and the pipeline completes normally.
    public ValueTask PublishAsync(AuditEvent value, CancellationToken ct = default) =>
        _events.Writer.WriteAsync(value, ct);

    public IAsyncEnumerable<AuditEvent> ReadAllAsync(CancellationToken ct = default) =>
        _events.Reader.ReadAllAsync(ct);

    public void Complete() => _events.Writer.TryComplete();
}
