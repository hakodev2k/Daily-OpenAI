record Audit(string TenantId, int Sequence, string Message);
var partitions = Enumerable.Range(1, 8).ToDictionary(p => $"tenant-{p}", p => Enumerable.Range(1, 20).Select(i => new Audit($"tenant-{p}", i, $"event-{i}")).ToList());
var targetTenant = "tenant-4";
var selected = partitions[targetTenant];
var items = selected.Where(x => x.TenantId == targetTenant).ToList();
Console.WriteLine($"records={items.Count}");
Console.WriteLine("partitionsTouched=1");
Console.WriteLine($"scanned={selected.Count}");
Console.WriteLine($"requestUnits={2 + selected.Count / 10}");