using System.Diagnostics;

static async Task CallDependencyAsync(CancellationToken cancellationToken)
{
    await Task.Delay(800, cancellationToken);
}

const int maxAttempts = 3;
var stopwatch = Stopwatch.StartNew();
var attempts = 0;

for (var attempt = 1; attempt <= maxAttempts; attempt++)
{
    attempts++;
    using var attemptTimeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));

    try
    {
        await CallDependencyAsync(attemptTimeout.Token);
        Console.WriteLine("RESULT=success");
        break;
    }
    catch (OperationCanceledException) when (attemptTimeout.IsCancellationRequested)
    {
        Console.WriteLine($"ATTEMPT_{attempt}=timeout");
    }
}

stopwatch.Stop();
Console.WriteLine($"ATTEMPTS={attempts}");
Console.WriteLine($"ELAPSED_MS={stopwatch.ElapsedMilliseconds}");
