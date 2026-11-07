using DronePermits;

return (args.FirstOrDefault() ?? "inspect").ToLowerInvariant() switch
{
    "inspect" => Inspect(),
    "reproduce" => Reproduce(),
    "verify" => Verify(),
    _ => InvalidMode()
};

int Inspect()
{
    try
    {
        var result = RunScenario();
        Print(result);
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"INSPECT_FAILED {ex.GetType().Name}: {ex.Message}");
        return 2;
    }
}

int Reproduce()
{
    try
    {
        var result = RunScenario();
        Print(result);
        if (result.IssuedAfterRace >= 2 && result.SameCachedPermit && result.ReusedPermit)
        {
            Console.WriteLine("EXPECTED_SYMPTOM: duplicate external issuance for one cache key");
            return 0;
        }
        Console.Error.WriteLine("REPRODUCE_FAILED: expected symptom not observed");
        return 1;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"REPRODUCE_FAILED {ex.GetType().Name}: {ex.Message}");
        return 2;
    }
}

int Verify()
{
    try
    {
        var result = RunScenario();
        Print(result);
        Assert(result.IssuedAfterRace == 1, "same-key concurrent requests must issue once");
        Assert(result.SameCachedPermit, "both callers must receive the same cached permit");
        Assert(result.ReusedPermit, "later same-key request must reuse the cached permit");
        Assert(result.IssuedAfterOtherKey == 2, "a different corridor must issue independently");
        Console.WriteLine("VERIFY_PASS: one side effect per key and stable cache behavior");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"VERIFY_FAIL {ex.GetType().Name}: {ex.Message}");
        return 1;
    }
}

ScenarioResult RunScenario()
{
    var authority = new FakePermitAuthority();
    var registry = new PermitRegistry(authority.Issue);
    using var ready = new CountdownEvent(2);
    using var start = new ManualResetEventSlim(false);

    Task<Permit> StartWorker() => Task.Factory.StartNew(() =>
    {
        ready.Signal();
        start.Wait();
        return registry.GetOrIssue("CORRIDOR-42");
    }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

    var workers = new[] { StartWorker(), StartWorker() };
    if (!ready.Wait(TimeSpan.FromSeconds(5)))
    {
        authority.Release();
        throw new TimeoutException("Workers did not reach the starting gate");
    }

    start.Set();
    // Let both external attempts reach the fake authority when the starter races.
    // A corrected implementation may legitimately issue only once.
    SpinWait.SpinUntil(() => authority.IssuedCount >= 2, TimeSpan.FromSeconds(3));
    authority.Release();

    if (!Task.WaitAll(workers, TimeSpan.FromSeconds(5)))
        throw new TimeoutException("Workers did not complete");

    var afterRace = authority.IssuedCount;
    var same = ReferenceEquals(workers[0].Result, workers[1].Result);
    var reused = ReferenceEquals(workers[0].Result, registry.GetOrIssue("CORRIDOR-42"));
    _ = registry.GetOrIssue("CORRIDOR-99");
    return new ScenarioResult(afterRace, same, reused, authority.IssuedCount);
}

void Print(ScenarioResult result) =>
    Console.WriteLine($"OBSERVED issued={result.IssuedAfterRace}; sameCachedPermit={result.SameCachedPermit}; reusedPermit={result.ReusedPermit}; issuedAfterOtherKey={result.IssuedAfterOtherKey}");

void Assert(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

int InvalidMode()
{
    Console.Error.WriteLine("Usage: dotnet run -- [inspect|reproduce|verify]");
    return 2;
}

internal readonly record struct ScenarioResult(
    int IssuedAfterRace, bool SameCachedPermit, bool ReusedPermit, int IssuedAfterOtherKey);

internal sealed class FakePermitAuthority
{
    private int _issuedCount;
    private readonly ManualResetEventSlim _release = new(false);

    public int IssuedCount => Volatile.Read(ref _issuedCount);

    public Permit Issue(string corridorId)
    {
        var number = Interlocked.Increment(ref _issuedCount);
        if (!_release.Wait(TimeSpan.FromSeconds(10)))
            throw new TimeoutException("Fake authority was not released");
        return new Permit(corridorId, $"PERMIT-{number:000}");
    }

    public void Release() => _release.Set();
}
