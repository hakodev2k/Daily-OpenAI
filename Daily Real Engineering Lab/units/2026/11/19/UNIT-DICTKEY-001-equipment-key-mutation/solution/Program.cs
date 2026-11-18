using System;
using System.Collections.Generic;

var key = new EquipmentKey("STATION-7", "PUMP-041");
var registry = new Dictionary<EquipmentKey, DeviceRecord> { [key] = new("inspection-open", "PUMP-041") };
Console.WriteLine($"before={registry.TryGetValue(key, out _)} count={registry.Count}");
registry[key].CurrentAssetTag = "PUMP-041-REPLACED";
Console.WriteLine($"after={registry.TryGetValue(key, out _)} count={registry.Count}");
Console.WriteLine($"remove={registry.Remove(key)} count={registry.Count}");

public sealed record EquipmentKey(string Site, string OriginalAssetTag);
public sealed class DeviceRecord
{
    public string Status { get; }
    public string CurrentAssetTag { get; set; }
    public DeviceRecord(string status, string currentAssetTag) => (Status, CurrentAssetTag) = (status, currentAssetTag);
}
