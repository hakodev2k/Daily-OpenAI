using System.Threading;

var active = 0;
var maxActive = 0;
var executions = 0;

using var timer = new Timer(async _ =>
{
    var nowActive = Interlocked.Increment(ref active);
    InterlockedExtensions.Max(ref maxActive, nowActive);
    var id = Interlocked.Increment(ref executions);
    Console.WriteLine($"START id={id} active={nowActive} at={DateTimeOffset.UtcNow:O}");

    await Task.Delay(250);

    nowActive = Interlocked.Decrement(ref active);
    Console.WriteLine($"END   id={id} active={nowActive} at={DateTimeOffset.UtcNow:O}");
}, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));

await Task.Delay(900);
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
