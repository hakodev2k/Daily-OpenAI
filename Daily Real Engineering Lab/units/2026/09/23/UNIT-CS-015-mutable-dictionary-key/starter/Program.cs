using System.Diagnostics.CodeAnalysis;

var rule = new PromotionRule("VIP", 10);
var evaluated = new Dictionary<PromotionRule, string>();
evaluated[rule] = "accepted";

var before = evaluated.TryGetValue(rule, out _);
rule.DiscountPercent = 20; // legitimate business update during recalculation
var after = evaluated.TryGetValue(rule, out _);
var enumerated = evaluated.Keys.Contains(rule, ReferenceEqualityComparer.Instance);

Console.WriteLine($"before={before}; after={after}; count={evaluated.Count}; enumeratedSameReference={enumerated}; hash={rule.GetHashCode()}");

if (args.Contains("--verify"))
{
    if (!before || !after || !enumerated || evaluated.Count != 1)
        throw new InvalidOperationException("Lookup invariant is not stable across the business update.");
    Console.WriteLine("VERIFY_OK");
}
else
{
    if (before && !after && enumerated)
    {
        Console.WriteLine("REPRODUCED");
        return;
    }
    throw new InvalidOperationException("Expected reproduction symptom was not observed.");
}

public sealed class PromotionRule : IEquatable<PromotionRule>
{
    public PromotionRule(string code, int discountPercent) => (Code, DiscountPercent) = (code, discountPercent);
    public string Code { get; }
    public int DiscountPercent { get; set; }
    public bool Equals(PromotionRule? other) => other is not null && Code == other.Code && DiscountPercent == other.DiscountPercent;
    public override bool Equals(object? obj) => obj is PromotionRule other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Code, DiscountPercent);
}

public sealed class ReferenceEqualityComparer : IEqualityComparer<PromotionRule>
{
    public static readonly ReferenceEqualityComparer Instance = new();
    public bool Equals(PromotionRule? x, PromotionRule? y) => ReferenceEquals(x, y);
    public int GetHashCode([DisallowNull] PromotionRule obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
}