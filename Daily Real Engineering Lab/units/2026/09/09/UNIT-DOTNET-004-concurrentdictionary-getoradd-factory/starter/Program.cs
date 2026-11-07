using System.Collections.Concurrent;

namespace TenantRegistryLab;

internal static class Program
{
    public static int Main(string[] args)
    {
        var mode = args.SingleOrDefault() ?? "--run";
        var result = ScenarioRunner.Run();

        Console.WriteLine($"Same instance: {ReferenceEquals(result.First, result.Second)}");
        Console.WriteLine($"Registry entries: {result.RegistryEntries}");
        Console.WriteLine($"Expensive initializations: {result.CreatedClientCount}");
        Console.WriteLine($"Client ordinals returned: {result.First.Ordinal}, {result.Second.Ordinal}");

        return mode switch
        {
            "--run" => 0,
            "--reproduce" => Reproduce(result),
            "--verify" => Verify(result),
            _ => UnknownMode(mode)
        };
    }

    private static int Reproduce(ScenarioResult result)
    {
        var symptomObserved =
            ReferenceEquals(result.First, result.Second) &&
            result.RegistryEntries == 1 &&
            result.CreatedClientCount >= 2;

        Console.WriteLine(symptomObserved
            ? "REPRODUCED: one retained entry, but initialization executed multiple times."
            : "NOT REPRODUCED: expected contention symptom was not observed.");

        return symptomObserved ? 0 : 1;
    }

    private static int Verify(ScenarioResult result)
    {
        var fixedBehavior =
            ReferenceEquals(result.First, result.Second) &&
            result.RegistryEntries == 1 &&
            result.CreatedClientCount == 1;

        Console.WriteLine(fixedBehavior
            ? "VERIFIED: callers share one client and initialization ran exactly once."
            : "VERIFY FAILED: the per-key initialization invariant is not satisfied.");

        return fixedBehavior ? 0 : 1;
    }

    private static int UnknownMode(string mode)
    {
        Console.Error.WriteLine($"Unknown mode: {mode}");
        return 2;
    }
}

internal static class ScenarioRunner
{
    public static ScenarioResult Run()
    {
        var registry = new TenantClientRegistry();
        var callers = Enumerable.Range(0, 2)
            .Select(_ => Task.Run(() => registry.GetOrCreate("tenant-acme")))
            .ToArray();

        Task.WhenAll(callers).GetAwaiter().GetResult();

        return new ScenarioResult(
            callers[0].Result,
            callers[1].Result,
            registry.Count,
            registry.CreatedClientCount);
    }
}

internal sealed class TenantClientRegistry
{
    private readonly ConcurrentDictionary<string, TenantClient> _clients =
        new(StringComparer.Ordinal);

    private readonly FactoryContentionGate _gate = new(participants: 2);
    private int _createdClientCount;

    public int Count => _clients.Count;
    public int CreatedClientCount => Volatile.Read(ref _createdClientCount);

    public TenantClient GetOrCreate(string tenantId)
    {
        return _clients.GetOrAdd(tenantId, CreateClient);
    }

    private TenantClient CreateClient(string tenantId)
    {
        // Investigation note:
        // This gate only makes simultaneous cache misses easier to observe.
        // Do not assume the collection's callback contract before verifying it.
        _gate.Arrive();

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

internal sealed record ScenarioResult(
    TenantClient First,
    TenantClient Second,
    int RegistryEntries,
    int CreatedClientCount);
