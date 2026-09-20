using System.Diagnostics.CodeAnalysis;

var rules = new Dictionary<RuleKey, decimal>();
var key = new RuleKey("vn", "retail");
rules[key] = 0.15m;

Console.WriteLine($"count-before={rules.Count}");
Console.WriteLine($"lookup-before={rules.ContainsKey(new RuleKey("vn", "retail"))}");
Console.WriteLine($"hash-before={key.GetHashCode()}");

// A legacy normalization step mutates the object reused by later pipeline stages.
key.Market = key.Market.ToUpperInvariant();

Console.WriteLine($"hash-after={key.GetHashCode()}");
Console.WriteLine($"count-after={rules.Count}");
Console.WriteLine($"lookup-after={rules.ContainsKey(new RuleKey("VN", "retail"))}");

public sealed class RuleKey : IEquatable<RuleKey>
{
    public RuleKey(string market, string channel) => (Market, Channel) = (market, channel);
    public string Market { get; set; }
    public string Channel { get; set; }

    public bool Equals(RuleKey? other) => other is not null &&
        StringComparer.OrdinalIgnoreCase.Equals(Market, other.Market) &&
        StringComparer.OrdinalIgnoreCase.Equals(Channel, other.Channel);

    public override bool Equals(object? obj) => Equals(obj as RuleKey);
    public override int GetHashCode() => HashCode.Combine(
        StringComparer.OrdinalIgnoreCase.GetHashCode(Market),
        StringComparer.OrdinalIgnoreCase.GetHashCode(Channel));
}