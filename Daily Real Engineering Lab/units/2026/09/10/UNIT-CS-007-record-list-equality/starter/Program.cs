using System.Collections.Generic;

var batch1 = new ProductBatch("tenant-a", new List<int> { 10, 20, 30 });
var batch2 = new ProductBatch("tenant-a", new List<int> { 10, 20, 30 });

Console.WriteLine($"LOGICAL_DATA_SAME={batch1.ProductIds.SequenceEqual(batch2.ProductIds)}");
Console.WriteLine($"RECORD_EQUAL={batch1 == batch2}");

var set = new HashSet<ProductBatch> { batch1, batch2 };
Console.WriteLine($"HASHSET_COUNT={set.Count}");

// Investigation note:
// Which members participate in generated record equality, and what equality semantics do those members provide?
public sealed record ProductBatch(string TenantId, List<int> ProductIds);
