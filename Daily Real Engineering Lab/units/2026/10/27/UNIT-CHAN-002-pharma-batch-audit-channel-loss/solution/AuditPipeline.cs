using System.Threading.Channels;

namespace AuditLab;

public sealed record AuditRecord(long Sequence, string BatchId, string Action);

public sealed class AuditPipeline
{
    private readonly Channel<AuditRecord> _events;

    public AuditPipeline(int capacity)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));

        _events = Channel.CreateBounded<AuditRecord>(new BoundedChannelOptions(capacity)
        {
            SingleWriter = true,
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    public async Task PublishAsync(IReadOnlyList<AuditRecord> records, CancellationToken cancellationToken)
    {
        foreach (var record in records)
        {
            await _events.Writer.WriteAsync(record, cancellationToken);
        }
    }

    public async Task<List<AuditRecord>> ReadAllAsync(CancellationToken cancellationToken)
    {
        var received = new List<AuditRecord>();
        await foreach (var record in _events.Reader.ReadAllAsync(cancellationToken))
        {
            received.Add(record);
        }

        return received;
    }

    public void Complete() => _events.Writer.TryComplete();
}
