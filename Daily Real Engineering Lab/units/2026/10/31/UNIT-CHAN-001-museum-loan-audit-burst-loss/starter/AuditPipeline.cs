using System.Threading.Channels;

namespace MuseumLoanAudit;

public sealed record AuditEvent(int Sequence, string ObjectCode);
public sealed record AuditRunResult(int Submitted, IReadOnlyList<int> StoredSequences);

public static class AuditPipeline
{
    public static async Task<AuditRunResult> RunAsync(
        IReadOnlyList<AuditEvent> events,
        Func<AuditEvent, CancellationToken, ValueTask> persist,
        CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateBounded<AuditEvent>(
            new BoundedChannelOptions(3)
            {
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });

        int admitted = 0;
        foreach (var auditEvent in events)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (channel.Writer.TryWrite(auditEvent))
                admitted++;
        }

        channel.Writer.TryComplete();
        var stored = new List<int>();
        await foreach (var auditEvent in channel.Reader.ReadAllAsync(cancellationToken))
        {
            await persist(auditEvent, cancellationToken);
            stored.Add(auditEvent.Sequence);
        }

        return new AuditRunResult(admitted, stored);
    }
}
