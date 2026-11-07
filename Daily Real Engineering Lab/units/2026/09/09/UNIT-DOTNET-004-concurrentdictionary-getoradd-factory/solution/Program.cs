using System.Collections.Concurrent;

namespace TenantRegistrySolution;

internal static class Program
{
    public static int Main()
    {
        var registry = new TenantClientRegistry();
        var callers = Enumerable.Range(0, 2)
            .Select(_ => Task.Run(() => registry.GetOrCreate("tenant-acme")))
            .ToArray();

        Task.WhenAll(callers).GetAwaiter().GetResult();

        var sameInstance = ReferenceEquals(callers[0].Result, callers[1].Result);
        Console.WriteLine($"Same instance: {sameInstance}");
        Console.WriteLine($"Registry entries: {registry.Count}");
        Console.WriteLine($"Expensive initializations: {registry.CreatedClientCount}");

        var passed = sameInstance && registry.Count == 1 && registry.CreatedClientCount == 1;
        Console.WriteLine(passed ? "REFERENCE CHECK PASSED" : "REFERENCE CHECK FAILED");
        return passed ? 0 : 1;
    }
}

internal sealed class TenantClientRegistry
{
    private readonly ConcurrentDictionary<string, Lazy<TenantClient>> _clients =
        new(StringComparer.Ordinal);

    private readonly FactoryContentionGate _gate = new(participants: 2);
    private int _createdClientCount;

    public int Count => _clients.Count;
    public int CreatedClientCount => Volatile.Read(ref _createdClientCount);

    public TenantClient GetOrCreate(string tenantId)
    {
        return _clients.GetOrAdd(tenantId, CreateLazyCandidate).Value;
    }

    private Lazy<TenantClient> CreateLazyCandidate(string tenantId)
    {
        _gate.Arrive();

        return new Lazy<TenantClient>(
            () => CreateClient(tenantId),
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private TenantClient CreateClient(string tenantId)
    {
        var ordinal = Interlocked.Increment(ref _createdClientCount);
        Thread.Sleep(25);
        return new TenantClient(tenantId, ordinal);
    }
}

internal sealed class FactoryContentionGate(int participants)
{
    private readonly Barrier _barrier = new(participants);

    public void Arrive()
    {
        _barrier.SignalAndWait(TimeSpan.FromSeconds(2));
    }
}

internal sealed record TenantClient(string TenantId, int Ordinal);
