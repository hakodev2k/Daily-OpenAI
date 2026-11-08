using System;
using System.Collections.Generic;

var asset = new ToolAsset("TOOL-104", "Cordless drill");
var reservations = new Dictionary<ToolAsset, string>();
reservations[asset] = "RES-9001";

bool before = reservations.TryGetValue(asset, out var first) && first == "RES-9001";
asset.DisplayCode = "TOOL-104-R"; // inventory label changed by staff
bool after = reservations.TryGetValue(asset, out var second) && second == "RES-9001";
Console.WriteLine($"count={reservations.Count}; before={before}; after={after}");
if (!before || !after || reservations.Count != 1) Environment.ExitCode = 2;

public sealed class ToolAsset : IEquatable<ToolAsset>
{
    public ToolAsset(string assetId, string name) { AssetId = assetId; Name = name; DisplayCode = assetId; }
    public string AssetId { get; }
    public string Name { get; }
    public string DisplayCode { get; set; }
    public bool Equals(ToolAsset? other) => other is not null && DisplayCode == other.DisplayCode;
    public override bool Equals(object? obj) => obj is ToolAsset other && Equals(other);
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(DisplayCode);
}
