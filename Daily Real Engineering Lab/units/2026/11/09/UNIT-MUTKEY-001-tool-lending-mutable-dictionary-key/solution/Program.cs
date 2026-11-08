using System;
using System.Collections.Generic;

var asset = new ToolAsset("TOOL-104", "Cordless drill");
var reservations = new Dictionary<string, string>(StringComparer.Ordinal);
reservations[asset.AssetId] = "RES-9001";
bool before = reservations.TryGetValue(asset.AssetId, out var first) && first == "RES-9001";
asset.DisplayCode = "TOOL-104-R";
bool after = reservations.TryGetValue(asset.AssetId, out var second) && second == "RES-9001";
Console.WriteLine($"count={reservations.Count}; before={before}; after={after}");
if (!before || !after || reservations.Count != 1) Environment.ExitCode = 2;

public sealed class ToolAsset
{
    public ToolAsset(string assetId, string name) { AssetId = assetId; Name = name; DisplayCode = assetId; }
    public string AssetId { get; }
    public string Name { get; }
    public string DisplayCode { get; set; }
}
