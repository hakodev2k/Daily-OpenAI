using System.Diagnostics;

// Synthetic sensor: a single slow cycle models a camera gateway stall.
using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
using var stop = new CancellationTokenSource(TimeSpan.FromMilliseconds(1150));
var clock = Stopwatch.StartNew();
var polls = 0;
try
{
    while (await timer.WaitForNextTickAsync(stop.Token))
    {
        polls++;
        Console.WriteLine($"poll={polls} elapsedMs={clock.ElapsedMilliseconds}");
        await Task.Delay(polls == 2 ? 460 : 20, stop.Token);
    }
}
catch (OperationCanceledException) when (stop.IsCancellationRequested) { }
var expectedSlots = clock.ElapsedMilliseconds / 100;
Console.WriteLine($"SUMMARY polls={polls} expectedSlots={expectedSlots}");
