using System.Diagnostics;

var overallStop = CancellationToken.None;
using var attemptTimeout = CancellationTokenSource.CreateLinkedTokenSource(overallStop);

for (var attempt = 1; attempt <= 2; attempt++)
{
    attemptTimeout.CancelAfter(TimeSpan.FromMilliseconds(120));
    var sw = Stopwatch.StartNew();

    try
    {
        Console.WriteLine($"Attempt {attempt} started. tokenCanceled={attemptTimeout.IsCancellationRequested}");
        var delay = attempt == 1 ? 250 : 30;
        await SimulatedSupplierCallAsync(delay, attemptTimeout.Token);
        Console.WriteLine($"Attempt {attempt} succeeded after {sw.ElapsedMilliseconds} ms");
        return 0;
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine($"Attempt {attempt} stopped after {sw.ElapsedMilliseconds} ms. tokenCanceled={attemptTimeout.IsCancellationRequested}");
    }
}

Console.WriteLine("All attempts failed.");
return 1;

static async Task SimulatedSupplierCallAsync(int delayMs, CancellationToken token)
{
    await Task.Delay(delayMs, token);
}
