record Order(string TenantId, string OrderId, decimal Total);

var partitions = Enumerable.Range(1, 12)
    .ToDictionary(i => $"tenant-{i:00}", i => Enumerable.Range(1, 20)
        .Select(n => new Order($"tenant-{i:00}", $"ORD-{i:00}-{n:000}", i * 100 + n))
        .ToList());

var tenantId = "tenant-07";
var orderId = "ORD-07-013";

// Investigation note: the caller already knows the tenant identity.
// Observe how much data this lookup path asks the storage layer to consider.
var scannedPartitions = 0;
var matches = new List<Order>();
foreach (var partition in partitions.Values)
{
    scannedPartitions++;
    matches.AddRange(partition.Where(x => x.TenantId == tenantId && x.OrderId == orderId));
}

var ruEstimate = 1.0 + scannedPartitions * 0.65;
Console.WriteLine($"PARTITIONS_SCANNED {scannedPartitions}");
Console.WriteLine($"ITEMS_RETURNED {matches.Count}");
Console.WriteLine($"RESULT {(matches.SingleOrDefault()?.OrderId ?? "NONE")}");
Console.WriteLine($"RU_ESTIMATE {ruEstimate:F2}");