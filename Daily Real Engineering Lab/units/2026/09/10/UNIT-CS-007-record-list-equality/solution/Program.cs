var batch1 = new ProductBatch("tenant-a", new[] { 10, 20, 30 });
var batch2 = new ProductBatch("tenant-a", new[] { 10, 20, 30 });
var reordered = new ProductBatch("tenant-a", new[] { 30, 20, 10 });

var set = new HashSet<ProductBatch> { batch1, batch2 };
Console.WriteLine($"EQUAL_SAME_SEQUENCE={batch1.Equals(batch2)}");
Console.WriteLine($"EQUAL_DIFFERENT_ORDER={batch1.Equals(reordered)}");
Console.WriteLine($"HASHSET_COUNT={set.Count}");

public sealed class ProductBatch : IEquatable<ProductBatch>
{
    public string TenantId { get; }
    public IReadOnlyList<int> ProductIds { get; }

    public ProductBatch(string tenantId, IEnumerable<int> productIds)
    {
        TenantId = tenantId;
        ProductIds = productIds.ToArray();
    }

    public bool Equals(ProductBatch? other) =>
        other is not null &&
        StringComparer.Ordinal.Equals(TenantId, other.TenantId) &&
        ProductIds.SequenceEqual(other.ProductIds);

    public override bool Equals(object? obj) => obj is ProductBatch other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(TenantId, StringComparer.Ordinal);
        foreach (var id in ProductIds) hash.Add(id);
        return hash.ToHashCode();
    }
}
