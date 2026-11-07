using System.Diagnostics;

var mode = args.FirstOrDefault() ?? "reproduce";
var dependency = new ShippingDependency();
var client = new ShippingQuoteClient(dependency);
var sw = Stopwatch.StartNew();
using var callerDeadline = new CancellationTokenSource(TimeSpan.FromMilliseconds(900));

try
{
    var quote = await client.GetQuoteAsync(mode, callerDeadline.Token);
    Console.WriteLine($"RESULT success quote={quote} elapsedMs={sw.ElapsedMilliseconds} attempts={dependency.Attempts}");
}
catch (Exception ex)
{
    Console.WriteLine($"RESULT failure type={ex.GetType().Name} elapsedMs={sw.ElapsedMilliseconds} attempts={dependency.Attempts}");
}

public sealed class ShippingQuoteClient(ShippingDependency dependency)
{
    public async Task<decimal> GetQuoteAsync(string mode, CancellationToken requestCancellation)
    {
        Exception? last = null;
        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                // Investigation note: compare the lifetime of this attempt with the caller's useful lifetime.
                using var attemptTimeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(700));
                return await dependency.GetQuoteAsync(mode, attempt, attemptTimeout.Token);
            }
            catch (OperationCanceledException ex)
            {
                last = ex;
                Console.WriteLine($"attempt={attempt} timed-out");
            }
        }
        throw last ?? new TimeoutException();
    }
}

public sealed class ShippingDependency
{
    public int Attempts { get; private set; }

    public async Task<decimal> GetQuoteAsync(string mode, int attempt, CancellationToken cancellationToken)
    {
        Attempts++;
        var delay = mode switch
        {
            "recover" when attempt == 1 => 500,
            "recover" => 80,
            "healthy" => 80,
            _ => 1500
        };
        Console.WriteLine($"dependency-start attempt={attempt} delayMs={delay}");
        await Task.Delay(delay, cancellationToken);
        Console.WriteLine($"dependency-end attempt={attempt}");
        return 12.50m;
    }
}