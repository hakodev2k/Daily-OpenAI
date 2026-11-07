using System.Collections.Concurrent;

namespace LibraryPickup;

public sealed record PickupJob(string RequestId, string DestinationBranch);

public sealed class PickupDispatcher
{
    public async Task<IReadOnlyList<string>> DispatchAsync(
        IReadOnlyList<PickupJob> jobs,
        Func<Task>? diagnosticCheckpoint = null)
    {
        var pending = new ConcurrentQueue<PickupJob>(jobs);
        var receipts = new ConcurrentBag<string>();

        async Task WorkerAsync()
        {
            if (!pending.TryDequeue(out var claimed))
                return;

            if (diagnosticCheckpoint is not null)
                await diagnosticCheckpoint();

            await Task.Yield();
            receipts.Add(claimed.RequestId);
        }

        await Task.WhenAll(WorkerAsync(), WorkerAsync());
        return receipts.OrderBy(x => x, StringComparer.Ordinal).ToArray();
    }
}
