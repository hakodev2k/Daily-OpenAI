var clock = new ScriptedClock(new DateTimeOffset(2026,11,18,10,0,0,TimeSpan.Zero),new DateTimeOffset(2026,11,18,9,59,58,TimeSpan.Zero));
var timer=new ScriptedMonotonicTimer(1000000,1125000);
var start=clock.UtcNow;
var begin=timer.Timestamp;
// Ticket processing completed.
var end=timer.Timestamp;
var completed=clock.UtcNow;
var duration=timer.ElapsedMs(begin,end);
Console.WriteLine($"StartedAt={start:O} FinishedAt={completed:O} DurationMs={duration} Verdict={(duration>=0?"PASS":"FAIL")}");
if(args.Contains("--check") && Math.Abs(duration-125)>0.001) Environment.ExitCode=1;
sealed class ScriptedClock(params DateTimeOffset[] values)
{
    private int index;
    public DateTimeOffset UtcNow=>values[Math.Min(index++,values.Length-1)];
}
sealed class ScriptedMonotonicTimer(long begin,long end)
{
    private int index;
    public long Timestamp=>index++==0?begin:end;
    public double ElapsedMs(long a,long b)=>(b-a)/1000d;
}
