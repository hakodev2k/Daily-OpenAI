sealed class RuleKey
{
    public string Region { get; set; }
    public string Segment { get; }
    public RuleKey(string region, string segment) => (Region, Segment) = (region, segment);
    public override bool Equals(object? obj) => obj is RuleKey other && Region == other.Region && Segment == other.Segment;
    public override int GetHashCode() => HashCode.Combine(Region, Segment);
    public override string ToString() => $"{Region}/{Segment}";
}

var key = new RuleKey("vn", "retail");
var rules = new Dictionary<RuleKey, decimal> { [key] = 0.15m };
Console.WriteLine($"before count={rules.Count} found={rules.ContainsKey(key)} key={key}");

key.Region = "VN";
Console.WriteLine($"after  count={rules.Count} found={rules.ContainsKey(key)} key={key}");

if (args.Contains("--reproduce"))
    return rules.Count == 1 && !rules.ContainsKey(key) ? 0 : 2;

if (args.Contains("--verify"))
{
    var sameBusinessKey = new RuleKey("VN", "retail");
    var otherBusinessKey = new RuleKey("VN", "wholesale");
    return rules.ContainsKey(sameBusinessKey) && !rules.ContainsKey(otherBusinessKey) ? 0 : 3;
}
return 0;