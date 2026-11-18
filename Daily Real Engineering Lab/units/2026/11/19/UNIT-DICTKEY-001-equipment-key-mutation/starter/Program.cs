using System;
using System.Collections.Generic;

// Registry for field maintenance; dispatchers may update a device's physical tag.
var device = new EquipmentKey("STATION-7", "PUMP-041");
var registry = new Dictionary<EquipmentKey, string> { [device] = "inspection-open" };
Console.WriteLine($"before={registry.TryGetValue(device, out _)} count={registry.Count}");
device.AssetTag = "PUMP-041-REPLACED";
Console.WriteLine($"after={registry.TryGetValue(device, out _)} count={registry.Count}");
Console.WriteLine($"remove={registry.Remove(device)} count={registry.Count}");

public sealed class EquipmentKey : IEquatable<EquipmentKey>
{
    public string Site { get; }
    public string AssetTag { get; set; }
    public EquipmentKey(string site, string assetTag) => (Site, AssetTag) = (site, assetTag);
    public bool Equals(EquipmentKey? other) => other is not null && Site == other.Site && AssetTag == other.AssetTag;
    public override bool Equals(object? obj) => obj is EquipmentKey other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Site, AssetTag);
}
