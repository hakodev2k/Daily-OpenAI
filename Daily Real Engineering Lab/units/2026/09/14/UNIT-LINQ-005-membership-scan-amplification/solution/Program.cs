var products = Enumerable.Range(0, 2_000)
    .Select(i => new Product(new SkuKey(i)))
    .ToList();

var allowedSkus = Enumerable.Range(0, 1_000)
    .Select(i => new SkuKey(i * 2))
    .ToHashSet();

SkuKey.ResetComparisons();

var matched = products
    .Where(product => allowedSkus.Contains(product.Sku))
    .Count();

var comparisons = SkuKey.Comparisons;
Console.WriteLine($"Matched={matched}");
Console.WriteLine($"Comparisons={comparisons}");

var valid = matched == 1_000 && comparisons < 10_000;
Console.WriteLine(valid ? "VERIFY_PASS" : "VERIFY_FAIL");
return valid ? 0 : 1;

sealed record Product(SkuKey Sku);

sealed class SkuKey : IEquatable<SkuKey>
{
    private static long _comparisons;

    public SkuKey(int value) => Value = value;

    public int Value { get; }
    public static long Comparisons => Interlocked.Read(ref _comparisons);

    public static void ResetComparisons() => Interlocked.Exchange(ref _comparisons, 0);

    public bool Equals(SkuKey? other)
    {
        Interlocked.Increment(ref _comparisons);
        return other is not null && Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is SkuKey other && Equals(other);
    public override int GetHashCode() => Value;
    public override string ToString() => Value.ToString();
}
