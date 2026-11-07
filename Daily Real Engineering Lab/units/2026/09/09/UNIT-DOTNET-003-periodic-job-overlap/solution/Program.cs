using System.Threading;

var active = 0;
var maxActive = 0;
var executions = 0;

using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
using var stop = new CancellationTokenSource(TimeSpan.FromMilliseconds(950));

try
{
    while (await timer.WaitForNextTickAsync(stop.Token))
    {
        var nowActive = Interlocked.Increment(ref active);
        InterlockedExtensions.Max(ref maxActive, nowActive);
        var id = Interlocked.Increment(ref executions);
        Console.WriteLine($"START id={id} active={nowActive} at={DateTimeOffset.UtcNow:O}");

        try
        {
            await Task.Delay(250, stop.Token);
        }
        finally
        {
            nowActive = Interlocked.Decrement(ref active);
            Console.WriteLine($"END   id={id} active={nowActive} at={DateTimeOffset.UtcNow:O}");
        }
    }
}
catch (OperationCanceledException) when (stop.IsCancellationRequested)
{
}

Console.WriteLine($"SUMMARY executions={executions} maxActive={maxActive}");

static class InterlockedExtensions
{
    public static void Max(ref int location, int value)
    {
        int current;
        do
        {
            current = Volatile.Read(ref location);
            if (current >= value) return;
        }
        while (Interlocked.CompareExchange(ref location, value, current) != current);
    }
}
