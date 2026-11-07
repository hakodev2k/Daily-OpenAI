using System.Runtime.CompilerServices;

namespace SeatFeed;

public sealed record SeatUpdate(string SeatId, string Status);

public static class SeatAvailabilityFeed
{
    public static async IAsyncEnumerable<SeatUpdate> WatchAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var snapshots = new[]
        {
            new SeatUpdate("A01", "Available"),
            new SeatUpdate("A02", "Reserved"),
            new SeatUpdate("A03", "Available")
        };

        foreach (var snapshot in snapshots)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            yield return snapshot;
        }
    }
}
