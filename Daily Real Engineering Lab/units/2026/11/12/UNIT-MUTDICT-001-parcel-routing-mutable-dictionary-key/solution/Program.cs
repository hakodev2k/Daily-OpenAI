using System;
using System.Collections.Generic;

public sealed record Parcel(string ParcelId, string Route);

public static class Program
{
    public static int Main(string[] args)
    {
        var manifest = new Dictionary<string, Parcel>(StringComparer.Ordinal)
        {
            ["PK-102"] = new Parcel("PK-102", "NORTH")
        };
        Console.WriteLine($"BEFORE count={manifest.Count} found={manifest.ContainsKey("PK-102")}");
        manifest["PK-102"] = manifest["PK-102"] with { Route = "EAST" };
        Console.WriteLine($"AFTER count={manifest.Count} found={manifest.ContainsKey("PK-102")}");
        bool found = manifest.TryGetValue("PK-102", out var parcel);
        Console.WriteLine($"LOOKUP found={found} route={parcel?.Route ?? "<missing>"}");
        bool passed = found && parcel?.Route == "EAST" && manifest.Count == 1;
        Console.WriteLine(passed ? "PASS: corrected route is searchable" : "FAIL: parcel not retrievable with corrected route");
        return passed ? 0 : 1;
    }
}
