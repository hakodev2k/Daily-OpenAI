using System.Collections.Generic;

var cache = new Dictionary<string, Forecast>();
int upstreamCalls = 0;
Forecast Get(string stationId, DateOnly date)
{
    var key = $"{stationId.Trim().ToUpperInvariant()}|{date:yyyy-MM-dd}";
    if (cache.TryGetValue(key, out var cached))
    {
        Console.WriteLine($"HIT station={stationId} key={key} responseStation={cached.StationId}");
        return cached;
    }
    upstreamCalls++;
    var result = new Forecast(stationId, date, stationId == "NORTH" ? 12 : 29);
    cache[key] = result;
    Console.WriteLine($"MISS station={stationId} key={key} responseStation={result.StationId}");
    return result;
}
var day = new DateOnly(2026, 11, 10);
var north = Get("NORTH", day);
var south = Get("SOUTH", day);
var northNext = Get("NORTH", day.AddDays(1));
var northAgain = Get("NORTH", day);
bool isolated = north.StationId == "NORTH" && south.StationId == "SOUTH";
bool distinctDay = northNext.Date != north.Date;
bool reused = upstreamCalls == 3;
Console.WriteLine($"isolation={isolated} differentDay={distinctDay} cacheReuse={reused} upstreamCalls={upstreamCalls}");
var mode = args.Length > 0 ? args[0] : "--verify";
if (mode == "--reproduce")
{
    if (isolated) { Console.Error.WriteLine("Expected initial cross-station symptom was not reproduced."); return 1; }
    Console.WriteLine("REPRODUCED: cross-station response contamination"); return 0;
}
if (!isolated || !distinctDay || !reused || northAgain.StationId != "NORTH")
{
    Console.Error.WriteLine("VERIFY FAILED: station/date isolation or cache reuse invariant violated"); return 1;
}
Console.WriteLine("VERIFY PASS"); return 0;
record Forecast(string StationId, DateOnly Date, int TemperatureC);
