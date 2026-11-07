using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OutboxLab;

return await IncidentHarness.ExecuteAsync(args);

internal static class IncidentHarness
{
    public static async Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: --demo | --reproduce | --verify");
            return 2;
        }

        return args[0] switch
        {
            "--demo" => await DemonstrateAsync(expectIncident: false),
            "--reproduce" => await DemonstrateAsync(expectIncident: true),
            "--verify" => await VerifyAsync(),
            _ => 2
        };
    }

    private static OutboxMessage Message(long sequence) =>
        new(sequence, $"loyalty-{sequence}", $"customer-{sequence % 3}", "PointsCredited");

    private static async Task<int> DemonstrateAsync(bool expectIncident)
    {
        var store = new MemoryOutboxStore(new[] { Message(101), Message(102), Message(103) });
        var publisher = new FaultingPublisher(failOnceFor: new long[] { 102 });
        var dispatcher = new OutboxDispatcher(store, publisher, batchSize: 10);

        var first = await dispatcher.DispatchBatchAsync();
        Console.WriteLine($"Batch 1: attempted={first.Attempted}, acknowledged={first.Acknowledged}, " +
                          $"transientFailures={first.TransientFailures}, checkpoint={first.Checkpoint}");
        Console.WriteLine("Attempts after batch 1: " + string.Join(",", publisher.Attempts));
        Console.WriteLine("Delivered after batch 1: " + string.Join(",", publisher.Delivered));

        var second = await dispatcher.DispatchBatchAsync();
        Console.WriteLine($"Batch 2: attempted={second.Attempted}, acknowledged={second.Acknowledged}, " +
                          $"checkpoint={second.Checkpoint}");
        Console.WriteLine("Delivered after batch 2: " + string.Join(",", publisher.Delivered));

        bool incident = !publisher.Delivered.Contains(102) &&
                        publisher.Delivered.Contains(103) && store.Checkpoint == 103;
        Console.WriteLine(incident ? "INCIDENT_REPRODUCED" : "INCIDENT_NOT_REPRODUCED");
        return expectIncident && !incident ? 1 : 0;
    }

    private static async Task<int> VerifyAsync()
    {
        var failed = 0;
        async Task Case(string name, Func<Task> body)
        {
            try
            {
                await body();
                Console.WriteLine("PASS " + name);
            }
            catch (Exception exception)
            {
                failed++;
                Console.WriteLine("FAIL " + name + ": " + exception.Message);
            }
        }

        await Case("Transient failure does not skip earlier event", async () =>
        {
            var store = new MemoryOutboxStore(new[] { Message(101), Message(102), Message(103) });
            var publisher = new FaultingPublisher(new long[] { 102 });
            var dispatcher = new OutboxDispatcher(store, publisher, batchSize: 10);

            var first = await dispatcher.DispatchBatchAsync();
            Require(first.TransientFailures == 1, "First pass must observe injected 503.");
            Require(first.Attempted == 2, "A later event was attempted despite a gap.");
            Require(store.Checkpoint == 101, "Checkpoint must not skip undelivered event.");
            Require(publisher.Delivered.SequenceEqual(new long[] { 101 }),
                "Downstream must not receive a later event before the gap is resolved.");

            var second = await dispatcher.DispatchBatchAsync();
            Require(second.TransientFailures == 0, "One-shot fault must recover.");
            Require(store.Checkpoint == 103, "Retry did not advance through all events.");
            Require(publisher.Delivered.SequenceEqual(new long[] { 101, 102, 103 }),
                "Downstream events are missing, duplicated or out of order.");
        });

        await Case("Healthy batch and empty follow-up", async () =>
        {
            var store = new MemoryOutboxStore(new[] { Message(201), Message(202) });
            var publisher = new FaultingPublisher();
            var dispatcher = new OutboxDispatcher(store, publisher, batchSize: 10);
            var first = await dispatcher.DispatchBatchAsync();
            var second = await dispatcher.DispatchBatchAsync();
            Require(first.Attempted == 2 && first.Acknowledged == 2,
                "Healthy batch must publish all records.");
            Require(second.Attempted == 0 && store.Checkpoint == 202,
                "Empty follow-up must not republish acknowledged records.");
            Require(publisher.Delivered.SequenceEqual(new long[] { 201, 202 }),
                "Healthy delivery differs from outbox order.");
        });

        await Case("Batch boundary does not drop the next record", async () =>
        {
            var store = new MemoryOutboxStore(new[] { Message(301), Message(302), Message(303) });
            var publisher = new FaultingPublisher();
            var dispatcher = new OutboxDispatcher(store, publisher, batchSize: 2);
            await dispatcher.DispatchBatchAsync();
            Require(store.Checkpoint == 302 && publisher.Delivered.Count == 2,
                "First batch should stop at its configured limit.");
            await dispatcher.DispatchBatchAsync();
            Require(store.Checkpoint == 303 &&
                    publisher.Delivered.SequenceEqual(new long[] { 301, 302, 303 }),
                "Second batch should include the remaining record.");
        });

        await Case("Checkpoint write failure allows idempotent replay", async () =>
        {
            var store = new MemoryOutboxStore(new[] { Message(401) })
            {
                FailNextCheckpointWrite = true
            };
            var publisher = new FaultingPublisher();
            var dispatcher = new OutboxDispatcher(store, publisher);
            bool threw = false;
            try { await dispatcher.DispatchBatchAsync(); }
            catch (IOException) { threw = true; }
            Require(threw && store.Checkpoint == 0,
                "A failed checkpoint write must remain visible to the caller.");
            await dispatcher.DispatchBatchAsync();
            Require(store.Checkpoint == 401, "Replay must advance checkpoint.");
            Require(publisher.Attempts.SequenceEqual(new long[] { 401, 401 }),
                "Replay must retry a published but uncheckpointed event.");
            Require(publisher.Delivered.SequenceEqual(new long[] { 401 }),
                "Idempotent fake consumer should apply the event only once.");
        });

        Console.WriteLine(failed == 0 ? "ALL_VERIFICATIONS_PASSED" : $"VERIFICATION_FAILED: {failed}");
        return failed == 0 ? 0 : 1;
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

internal sealed class MemoryOutboxStore : IOutboxStore
{
    private readonly OutboxMessage[] _messages;
    private long _checkpoint;

    public MemoryOutboxStore(IEnumerable<OutboxMessage> messages)
    {
        _messages = messages.OrderBy(x => x.Sequence).ToArray();
        if (_messages.Select(x => x.Sequence).Distinct().Count() != _messages.Length)
            throw new ArgumentException("Outbox sequences must be unique.", nameof(messages));
    }

    public long Checkpoint => _checkpoint;
    public bool FailNextCheckpointWrite { get; set; }

    public Task<long> ReadCheckpointAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_checkpoint);
    }

    public Task<IReadOnlyList<OutboxMessage>> ReadBatchAfterAsync(
        long exclusiveSequence, int limit, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<OutboxMessage> batch = _messages
            .Where(x => x.Sequence > exclusiveSequence)
            .Take(limit)
            .ToArray();
        return Task.FromResult(batch);
    }

    public Task WriteCheckpointAsync(long sequence, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (FailNextCheckpointWrite)
        {
            FailNextCheckpointWrite = false;
            throw new IOException("Injected durable-checkpoint write failure");
        }
        if (sequence < _checkpoint)
            throw new InvalidOperationException("Checkpoint cannot move backwards.");
        _checkpoint = sequence;
        return Task.CompletedTask;
    }
}

internal sealed class FaultingPublisher : IEventPublisher
{
    private readonly HashSet<long> _failOnceFor;
    private readonly HashSet<long> _alreadyFailed = new();
    private readonly HashSet<string> _acceptedEventIds = new(StringComparer.Ordinal);

    public FaultingPublisher(IEnumerable<long>? failOnceFor = null)
    {
        _failOnceFor = new HashSet<long>(failOnceFor ?? Array.Empty<long>());
    }

    public List<long> Attempts { get; } = new();
    public List<long> Delivered { get; } = new();

    public Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Attempts.Add(message.Sequence);
        if (_failOnceFor.Contains(message.Sequence) && _alreadyFailed.Add(message.Sequence))
            throw new TransientDeliveryException("Injected broker 503 for sequence " + message.Sequence);
        if (_acceptedEventIds.Add(message.EventId))
            Delivered.Add(message.Sequence);
        return Task.CompletedTask;
    }
}
