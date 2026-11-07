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
                FullMode = BoundedChannelFullMode.Wait
            });

        var stored = new List<int>();
        var consumer = Task.Run(async () =>
        {
            try
            {
                await foreach (var auditEvent in channel.Reader.ReadAllAsync(cancellationToken))
                {
                    await persist(auditEvent, cancellationToken);
                    stored.Add(auditEvent.Sequence);
                }
            }
            catch (Exception ex)
            {
                channel.Writer.TryComplete(ex);
                throw;
            }
        });

        try
        {
            foreach (var auditEvent in events)
                await channel.Writer.WriteAsync(auditEvent, cancellationToken);

            channel.Writer.TryComplete();
            await consumer;
            return new AuditRunResult(events.Count, stored);
        }
        catch
        {
            channel.Writer.TryComplete();
            await consumer;
            throw;
        }
    }
}
