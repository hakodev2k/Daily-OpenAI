using TransitLab;

static (string First, string Second, int Entries, bool SecondHit) Replay(string firstLanguage, string secondLanguage)
{
    var cache = new Dictionary<string, string>(StringComparer.Ordinal);
    string Get(string language, out bool hit)
    {
        var key = CacheKeyBuilder.Build("/alerts/42", language);
        hit = cache.TryGetValue(key, out var text);
        if (!hit)
        {
            text = language.Equals("vi", StringComparison.OrdinalIgnoreCase) ? "Cầu đóng" : "Bridge closed";
            cache[key] = text;
        }
        return text!;
    }
    var first = Get(firstLanguage, out _);
    var second = Get(secondLanguage, out var secondHit);
    return (first, second, cache.Count, secondHit);
}
static void Require(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}
var mode = args.Length == 0 ? "inspect" : args[0];
if (mode == "inspect" || mode == "reproduce")
{
    foreach (var (a,b) in new[] { ("vi","en"), ("en","vi") })
    {
        var r = Replay(a,b);
        Console.WriteLine($"{a}->{b}: first={r.First}; second={r.Second}; entries={r.Entries}; secondHit={r.SecondHit}");
    }
    if (mode == "reproduce")
    {
        var r = Replay("vi","en");
        Require(r.Second != "Bridge closed", "Starter no longer reproduces incorrect-language response.");
        Console.WriteLine("REPRODUCED: valid English request received wrong-language cached content.");
    }
    return;
}
if (mode != "verify") throw new ArgumentException("Use inspect, reproduce, or verify.");
foreach (var (a,b,expected) in new[] { ("vi","en","Bridge closed"), ("en","vi","Cầu đóng") })
{
    var r = Replay(a,b);
    Require(r.Second == expected, $"Wrong language for {a}->{b}: {r.Second}");
    Require(r.Entries == 2, $"Expected 2 variants, got {r.Entries}");
    Require(!r.SecondHit, "Second distinct language must not reuse first variant");
}
var k1 = CacheKeyBuilder.Build("/alerts/42", "VI");
var k2 = CacheKeyBuilder.Build(" /ALERTS/42 ", "vi");
var k3 = CacheKeyBuilder.Build("/alerts/42", "en");
Require(k1 == k2, "Equivalent language/route should share a cache key");
Require(k1 != k3, "Different languages must have different cache keys");
Require(CacheKeyBuilder.Build("/alerts/43", "vi") != k1, "Different route must remain separate");
Console.WriteLine("PASS: order-independent language isolation and canonicalization.");
