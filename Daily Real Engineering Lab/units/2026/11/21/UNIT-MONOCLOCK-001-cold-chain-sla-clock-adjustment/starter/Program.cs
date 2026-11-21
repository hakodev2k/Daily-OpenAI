using System;
using System.Diagnostics;

IClock clock = new ScriptedClock();
var service = new TransitSlaMonitor(clock);
var observed = service.Measure(new Shipment("BOX-42"), () => clock.Advance());
Console.WriteLine($"Shipment BOX-42: elapsed={observed.TotalMilliseconds}ms, expected=3000ms");
if (args.Length == 0) return;
if (args[0] == "--reproduce")
{
    if (observed.TotalMilliseconds != -2000) throw new Exception("Không tái hiện được triệu chứng gốc.");
    Console.WriteLine("REPRODUCED: hệ thống ghi nhận thời gian âm.");
}
else if (args[0] == "--verify")
{
    if (observed != TimeSpan.FromSeconds(3)) throw new Exception($"FAIL: elapsed={observed.TotalMilliseconds}ms; expected 3000ms.");
    var normal = new ScriptedClock(jumpBackward: false);
    var normalElapsed = new TransitSlaMonitor(normal).Measure(new Shipment("BOX-43"), normal.Advance);
    if (normalElapsed != TimeSpan.FromSeconds(3)) throw new Exception("FAIL: normal clock regression.");
    Console.WriteLine("PASS: backward adjustment and normal progression.");
}
else throw new ArgumentException("Use --reproduce or --verify.");

record Shipment(string Id);
interface IClock
{
    DateTimeOffset UtcNow { get; }
    long MonotonicMilliseconds { get; }
}
sealed class ScriptedClock(bool jumpBackward = true) : IClock
{
    private bool advanced;
    public DateTimeOffset UtcNow => new DateTimeOffset(2026, 11, 21, 10, 0, 0, TimeSpan.Zero)
        .AddSeconds(advanced ? (jumpBackward ? -2 : 3) : 0);
    public long MonotonicMilliseconds => advanced ? 3000 : 0;
    public void Advance() => advanced = true;
}
sealed class ProductionClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    public long MonotonicMilliseconds => (long)(Stopwatch.GetTimestamp() * (1000.0 / Stopwatch.Frequency));
}
sealed class TransitSlaMonitor(IClock clock)
{
    public TimeSpan Measure(Shipment shipment, Action transfer)
    {
        ArgumentNullException.ThrowIfNull(shipment);
        ArgumentNullException.ThrowIfNull(transfer);
        DateTimeOffset started = clock.UtcNow;
        transfer();
        DateTimeOffset completed = clock.UtcNow;
        return completed - started;
    }
}
