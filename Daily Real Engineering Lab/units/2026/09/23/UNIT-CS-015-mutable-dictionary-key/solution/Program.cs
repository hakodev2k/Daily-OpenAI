var rule = new PromotionRule("VIP", 10);
var evaluated = new Dictionary<PromotionRule, string>();
evaluated[rule] = "accepted";

var before = evaluated.TryGetValue(rule, out _);
rule.DiscountPercent = 20;
var after = evaluated.TryGetValue(rule, out _);

Console.WriteLine($"before={before}; after={after}; count={evaluated.Count}");
if (!before || !after || evaluated.Count != 1) throw new InvalidOperationException("Lookup invariant failed.");
Console.WriteLine("VERIFY_OK");

public sealed class PromotionRule : IEquatable<PromotionRule>
{
    public PromotionRule(string code, int discountPercent) => (Code, DiscountPercent) = (code, discountPercent);
    public string Code { get; }
    public int DiscountPercent { get; set; }
    public bool Equals(PromotionRule? other) => other is not null && Code == other.Code;
    public override bool Equals(object? obj) => obj is PromotionRule other && Equals(other);
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Code);
}