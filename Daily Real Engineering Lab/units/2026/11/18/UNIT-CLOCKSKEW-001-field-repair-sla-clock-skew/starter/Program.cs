var clock = new ScriptedClock(new DateTimeOffset(2026,11,18,10,0,0,TimeSpan.Zero),new DateTimeOffset(2026,11,18,9,59,58,TimeSpan.Zero));
var start=clock.UtcNow;
var completed=clock.UtcNow;
var duration=(completed-start).TotalMilliseconds;
Console.WriteLine($"StartedAt={start:O} FinishedAt={completed:O} DurationMs={duration} Verdict={(duration>=0?"PASS":"FAIL")}");
if(args.Contains("--check") && duration<0) Environment.ExitCode=1;
sealed class ScriptedClock(params DateTimeOffset[] values)
{
    private int index;
    public DateTimeOffset UtcNow=>values[Math.Min(index++,values.Length-1)];
}
