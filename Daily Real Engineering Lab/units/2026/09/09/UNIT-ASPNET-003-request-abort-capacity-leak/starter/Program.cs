using System.Diagnostics;

return args.FirstOrDefault()?.ToLowerInvariant() switch
{
    "reproduce" => await LabHarness.ReproduceAsync(),
    "verify" => await LabHarness.VerifyAsync(),
    "run" => await LabHarness.RunAsync(),
    _ => LabHarness.PrintUsage()
};

static class LabHarness
{
    public static int PrintUsage()
    {
        Console.WriteLine("Use: dotnet run --project starter/RequestAbortCapacityLeak.csproj -- [reproduce|verify|run]");
        return 1;
    }

    public static async Task<int> RunAsync()
    {
        var service = new ExportService(maxConcurrency: 2);
        using var client = new CancellationTokenSource();
        var task = service.GenerateAsync("export-demo", client.Token);
        await service.WaitUntilActiveAsync(1, TimeSpan.FromSeconds(1));
        client.Cancel();
        Console.WriteLine($"Client canceled: {client.IsCancellationRequested}");
        Console.WriteLine($"Active immediately after cancel: {service.ActiveOperations}");
        try { await task; } catch (OperationCanceledException) { }
        Console.WriteLine($"Active after task completes: {service.ActiveOperations}");
        return 0;
    }

    public static async Task<int> ReproduceAsync()
    {
        var service = new ExportService(maxConcurrency: 2);
        using var firstClient = new CancellationTokenSource();
        using var secondClient = new CancellationTokenSource();

        var first = service.GenerateAsync("export-A", firstClient.Token);
        var second = service.GenerateAsync("export-B", secondClient.Token);
        await service.WaitUntilActiveAsync(2, TimeSpan.FromSeconds(1));

        firstClient.Cancel();
        secondClient.Cancel();
        await Task.Delay(150);

        var activeAfterAbort = service.ActiveOperations;
        var slotsAfterAbort = service.AvailableSlots;
        var abandonedStillRunning = !first.IsCompleted || !second.IsCompleted;

        Console.WriteLine("=== Incident evidence ===");
        Console.WriteLine($"First client canceled:  {firstClient.IsCancellationRequested}");
        Console.WriteLine($"Second client canceled: {secondClient.IsCancellationRequested}");
        Console.WriteLine($"Active operations:       {activeAfterAbort}");
        Console.WriteLine($"Available slots:         {slotsAfterAbort}");
        Console.WriteLine($"Abandoned work running:  {abandonedStillRunning}");

        await IgnoreCancellationAsync(first);
        await IgnoreCancellationAsync(second);

        var reproduced = activeAfterAbort == 2 && slotsAfterAbort == 0 && abandonedStillRunning;
        Console.WriteLine(reproduced
            ? "REPRODUCED: client-aborted requests still occupy bounded capacity."
            : "NOT REPRODUCED: expected incident signature was not observed.");

        return reproduced ? 0 : 2;
    }

    public static async Task<int> VerifyAsync()
    {
        var service = new ExportService(maxConcurrency: 2);
        using var firstClient = new CancellationTokenSource();
        using var secondClient = new CancellationTokenSource();

        var first = service.GenerateAsync("export-A", firstClient.Token);
        var second = service.GenerateAsync("export-B", secondClient.Token);
        await service.WaitUntilActiveAsync(2, TimeSpan.FromSeconds(1));

        firstClient.Cancel();
        secondClient.Cancel();

        var released = await WaitUntilAsync(
            () => service.ActiveOperations == 0 && service.AvailableSlots == 2,
            TimeSpan.FromMilliseconds(600));

        await IgnoreCancellationAsync(first);
        await IgnoreCancellationAsync(second);

        using var healthyClient = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var stopwatch = Stopwatch.StartNew();
        var healthyResult = await service.GenerateAsync("healthy-export", healthyClient.Token);
        stopwatch.Stop();

        var healthySucceeded = healthyResult == "healthy-export:ready";
        var passed = released && healthySucceeded && service.ActiveOperations == 0 && service.AvailableSlots == 2;

        Console.WriteLine("=== Verification ===");
        Console.WriteLine($"Slots released after abort: {released}");
        Console.WriteLine($"Healthy request succeeded:  {healthySucceeded}");
        Console.WriteLine($"Healthy duration:           {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Final active operations:    {service.ActiveOperations}");
        Console.WriteLine($"Final available slots:      {service.AvailableSlots}");
        Console.WriteLine(passed ? "VERIFY PASSED" : "VERIFY FAILED");

        return passed ? 0 : 3;
    }

    private static async Task IgnoreCancellationAsync(Task task)
    {
        try { await task; }
        catch (OperationCanceledException) { }
    }

    private static async Task<bool> WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
    {
        var deadline = Stopwatch.StartNew();
        while (deadline.Elapsed < timeout)
        {
            if (condition()) return true;
            await Task.Delay(20);
        }
        return condition();
    }
}

sealed class ExportService
{
    private readonly SemaphoreSlim _slots;
    private int _activeOperations;

    public ExportService(int maxConcurrency) => _slots = new SemaphoreSlim(maxConcurrency, maxConcurrency);

    public int ActiveOperations => Volatile.Read(ref _activeOperations);
    public int AvailableSlots => _slots.CurrentCount;

    public async Task<string> GenerateAsync(string exportId, CancellationToken requestAborted)
    {
        using var budget = new OperationBudget(TimeSpan.FromSeconds(5), requestAborted);
        await _slots.WaitAsync(budget.Token);
        Interlocked.Increment(ref _activeOperations);

        try
        {
            return await ReportStore.BuildAsync(exportId, budget.Token);
        }
        finally
        {
            Interlocked.Decrement(ref _activeOperations);
            _slots.Release();
        }
    }

    public async Task WaitUntilActiveAsync(int expected, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < timeout)
        {
            if (ActiveOperations == expected) return;
            await Task.Delay(10);
        }

        throw new TimeoutException($"Expected {expected} active operations, observed {ActiveOperations}.");
    }
}

sealed class OperationBudget : IDisposable
{
    private readonly CancellationTokenSource _timeout;

    public OperationBudget(TimeSpan timeout, CancellationToken requestAborted)
    {
        _timeout = new CancellationTokenSource(timeout);
        Token = _timeout.Token;
    }

    public CancellationToken Token { get; }

    public void Dispose() => _timeout.Dispose();
}

static class ReportStore
{
    public static async Task<string> BuildAsync(string exportId, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(1200), cancellationToken);
        return $"{exportId}:ready";
    }
}
