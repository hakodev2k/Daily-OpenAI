using System;
using System.Collections.Generic;

public sealed record ParcelKey
{
    public required string ParcelId { get; init; }
    public required string Route { get; set; }
}
public sealed record Parcel(string ParcelId, string Route);

public static class Program
{
    public static int Main(string[] args)
    {
        var key = new ParcelKey { ParcelId = "PK-102", Route = "NORTH" };
        var manifest = new Dictionary<ParcelKey, Parcel>
        {
            [key] = new Parcel(key.ParcelId, key.Route)
        };
        Console.WriteLine($"BEFORE count={manifest.Count} found={manifest.ContainsKey(key)}");
        key.Route = "EAST";
        Console.WriteLine($"AFTER count={manifest.Count} found={manifest.ContainsKey(key)}");
        var lookup = new ParcelKey { ParcelId = "PK-102", Route = "EAST" };
        bool found = manifest.TryGetValue(lookup, out var parcel);
        Console.WriteLine($"LOOKUP found={found} route={parcel?.Route ?? "<missing>"}");
        bool passed = found && parcel?.Route == "EAST" && manifest.Count == 1;
        Console.WriteLine(passed ? "PASS: corrected route is searchable" : "FAIL: parcel not retrievable with corrected route");
        return passed ? 0 : 1;
    }
}
