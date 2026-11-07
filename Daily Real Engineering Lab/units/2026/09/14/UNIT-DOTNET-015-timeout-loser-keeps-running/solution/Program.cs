var provider = new SimulatedProvider();

var calls = Enumerable.Range(0, 6).Select(async _ =>
{
    try
    {
        await GetPriceWithTimeoutAsync(provider);
        return "success";
    }
    catch (TimeoutException)
    {
        return "timeout";
    }
});

var results = await Task.WhenAll(calls);
Console.WriteLine($"TIMEOUTS={results.Count(x => x == "timeout")}");
Console.WriteLine($"ACTIVE_AFTER_TIMEOUTS={provider.ActiveOperations}");

static async Task<decimal> GetPriceWithTimeoutAsync(SimulatedProvider provider)
{
    using var attemptCts = new CancellationTokenSource();
    var work = provider.GetPriceAsync(attemptCts.Token);
    var timeout = Task.Delay(TimeSpan.FromMilliseconds(100));

    var completed = await Task.WhenAny(work, timeout);
    if (completed != work)
    {
        attemptCts.Cancel();
        try
        {
            await work;
        }
        catch (OperationCanceledException) when (attemptCts.IsCancellationRequested)
        {
        }

        throw new TimeoutException("Pricing provider timed out.");
    }

    return await work;
}

sealed class SimulatedProvider
{
    private int _activeOperations;
    public int ActiveOperations => Volatile.Read(ref _activeOperations);

    public async Task<decimal> GetPriceAsync(CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _activeOperations);
        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(800), cancellationToken);
            return 42.50m;
        }
        finally
        {
            Interlocked.Decrement(ref _activeOperations);
        }
    }
}
