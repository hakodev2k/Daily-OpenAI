using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OutboxLab;

public sealed record OutboxMessage(long Sequence, string EventId, string CustomerId, string EventType);

public sealed class TransientDeliveryException : Exception
{
    public TransientDeliveryException(string message) : base(message) { }
}

public interface IOutboxStore
{
    Task<long> ReadCheckpointAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<OutboxMessage>> ReadBatchAfterAsync(
        long exclusiveSequence, int limit, CancellationToken cancellationToken);
    Task WriteCheckpointAsync(long sequence, CancellationToken cancellationToken);
}

public interface IEventPublisher
{
    Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken);
}

public sealed record DispatchReport(
    int Attempted, int Acknowledged, int TransientFailures, long Checkpoint);

public sealed class OutboxDispatcher
{
    private readonly IOutboxStore _store;
    private readonly IEventPublisher _publisher;
    private readonly int _batchSize;

    public OutboxDispatcher(IOutboxStore store, IEventPublisher publisher, int batchSize = 50)
    {
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _batchSize = batchSize;
    }

    public async Task<DispatchReport> DispatchBatchAsync(CancellationToken cancellationToken = default)
    {
        var checkpoint = await _store.ReadCheckpointAsync(cancellationToken);
        var messages = await _store.ReadBatchAfterAsync(
            checkpoint, _batchSize, cancellationToken);

        var attempted = 0;
        var acknowledged = 0;
        var transientFailures = 0;

        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            attempted++;
            try
            {
                await _publisher.PublishAsync(message, cancellationToken);
                acknowledged++;
                await _store.WriteCheckpointAsync(message.Sequence, cancellationToken);
            }
            catch (TransientDeliveryException)
            {
                transientFailures++;
                // The persisted watermark describes a contiguous acknowledged prefix.
                // Retain the failed sequence for a later retry; do not cross the gap.
                break;
            }
        }

        return new DispatchReport(
            attempted, acknowledged, transientFailures,
            await _store.ReadCheckpointAsync(cancellationToken));
    }
}
