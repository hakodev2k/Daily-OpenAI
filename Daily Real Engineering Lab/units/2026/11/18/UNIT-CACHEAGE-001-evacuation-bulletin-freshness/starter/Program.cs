using System;
var clock = new Clock();
var source = new Source();
var cache = new Cache(clock, source, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10));
string shown = "";
foreach (int minute in new[] { 0, 4, 8, 12, 16 })
{
    clock.Minute = minute;
    if (minute == 12) source.Value = "ZONE-B-CLOSED";
    shown = cache.Read();
    Console.WriteLine($"t={minute:00} displayed={shown} source={source.Value} fetches={source.Fetches}");
}
if (args.Length != 1) return 2;
if (args[0] == "reproduce")
{
    if (shown != "ZONE-A-OPEN" || source.Fetches != 1) return 1;
    Console.WriteLine("REPRODUCED: stale bulletin at minute 16");
    return 0;
}
if (args[0] != "verify") return 2;
if (shown != "ZONE-B-CLOSED" || source.Fetches != 2)
{
    Console.Error.WriteLine("VERIFY FAILED: freshness contract violated");
    return 1;
}
Console.WriteLine("VERIFY PASS: updated bulletin displayed");
return 0;

sealed class Clock { public int Minute { get; set; } }
sealed class Source
{
    public string Value { get; set; } = "ZONE-A-OPEN";
    public int Fetches { get; private set; }
    public string Fetch() { Fetches++; return Value; }
}
sealed class Cache(Clock clock, Source source, TimeSpan idleTtl, TimeSpan maxAge)
{
    private string? value;
    private int lastAccessMinute;
    public string Read()
    {
        if (value is null || clock.Minute - lastAccessMinute >= idleTtl.TotalMinutes)
            value = source.Fetch();
        lastAccessMinute = clock.Minute;
        return value;
    }
}
