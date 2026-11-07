record Audit(string TenantId, int Sequence, string Message);

var partitions = Enumerable.Range(1, 8)
    .ToDictionary(p => $"tenant-{p}", p => Enumerable.Range(1, 20)
        .Select(i => new Audit($"tenant-{p}", i, $"event-{i}"))
        .ToList());

var targetTenant = "tenant-4";

// Learner path: investigate why a tenant-scoped read does more work than expected.
var result = Query(partitions, targetTenant, partitionKey: null);

Console.WriteLine($"records={result.Items.Count}");
Console.WriteLine($"partitionsTouched={result.PartitionsTouched}");
Console.WriteLine($"scanned={result.Scanned}");
Console.WriteLine($"requestUnits={result.RequestUnits}");

static QueryResult Query(Dictionary<string, List<Audit>> partitions, string tenantId, string? partitionKey)
{
    var selected = partitionKey is null
        ? partitions.Values
        : new[] { partitions[partitionKey] };

    var touched = selected.Count();
    var scanned = selected.Sum(x => x.Count);
    var items = selected.SelectMany(x => x).Where(x => x.TenantId == tenantId).ToList();
    var ru = touched * 2 + scanned / 10;
    return new QueryResult(items, touched, scanned, ru);
}

record QueryResult(List<Audit> Items, int PartitionsTouched, int Scanned, int RequestUnits);