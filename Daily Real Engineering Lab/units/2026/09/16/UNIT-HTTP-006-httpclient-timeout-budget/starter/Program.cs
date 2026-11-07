using System.Diagnostics;

var fast = args.Contains("--fast");
var delays = fast ? new[] { 60, 70 } : new[] { 650, 650 };
var service = new ShippingSummaryService(new FakeDependency(delays));
var sw = Stopwatch.StartNew();
try
{
    var result = await service.GetSummaryAsync(CancellationToken.None);
    Console.WriteLine($"RESULT={result}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("RESULT=DEADLINE_EXCEEDED");
}
finally
{
    sw.Stop();
    Console.WriteLine($"ELAPSED_MS={sw.ElapsedMilliseconds}");
}

sealed class ShippingSummaryService(FakeDependency dependency)
{
    private static readonly TimeSpan BusinessBudget = TimeSpan.FromMilliseconds(900);

    public async Task<string> GetSummaryAsync(CancellationToken requestAborted)
    {
        // Investigation note: BusinessBudget describes the contract for the whole operation.
        // Which lifetime currently governs the two dependency calls below?
        var inventory = await dependency.CallAsync("inventory", requestAborted);
        var carrier = await dependency.CallAsync("carrier", requestAborted);
        return $"{inventory}+{carrier};budget={BusinessBudget.TotalMilliseconds:0}";
    }
}

sealed class FakeDependency(int[] delays)
{
    private int _index;
    public async Task<string> CallAsync(string name, CancellationToken cancellationToken)
    {
        var delay = delays[Math.Min(_index++, delays.Length - 1)];
        Console.WriteLine($"START {name} delay={delay}ms");
        await Task.Delay(delay, cancellationToken);
        Console.WriteLine($"END {name}");
        return name.ToUpperInvariant();
    }
}