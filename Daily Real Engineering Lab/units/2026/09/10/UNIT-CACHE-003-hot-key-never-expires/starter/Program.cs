using System.Globalization;

var mode = args.FirstOrDefault()?.ToLowerInvariant() ?? "run";

return mode switch
{
    "reproduce" => RunReproduction(),
    "verify" => RunVerification(),
    _ => RunDemo()
};

static int RunDemo()
{
    var result = SimulateHotKey();
    Print(result);
    return 0;
}

static int RunReproduction()
{
    var result = SimulateHotKey();
    Print(result);

    var symptomObserved = result.VersionAtMinute12 == "v1" && result.SourceVersionAtMinute12 == "v2";
    Console.WriteLine(symptomObserved
        ? "REPRODUCED: source=v2 nhưng hot cache key vẫn trả v1 tại phút 12."
        : "NOT REPRODUCED: starter không còn biểu hiện symptom mong đợi.");

    return symptomObserved ? 0 : 1;
}

static int RunVerification()
{
    var result = SimulateHotKey();
    Print(result);

    var freshnessBounded = result.VersionAtMinute12 == "v2";
    var reusePreserved = result.SourceReads <= 3;
    var stableAfterRefresh = result.VersionAtMinute13 == "v2";

    if (freshnessBounded && reusePreserved && stableAfterRefresh)
    {
        Console.WriteLine("VERIFY PASS: cache refreshes within the required bound and still reuses cached data.");
        return 0;
    }

    Console.WriteLine("VERIFY FAIL:");
    if (!freshnessBounded)
        Console.WriteLine("- Hot key vẫn stale quá lâu.");
    if (!reusePreserved)
        Console.WriteLine("- Fix làm mất lợi ích cache: source bị đọc quá nhiều lần.");
    if (!stableAfterRefresh)
        Console.WriteLine("- Giá trị sau refresh không ổn định.");

    return 1;
}

static SimulationResult SimulateHotKey()
{
    var clock = new FakeClock(DateTimeOffset.Parse("2026-09-10T00:00:00Z", CultureInfo.InvariantCulture));
    var source = new PricingRuleSource("v1");
    var cache = BuildCache(clock, source);

    var initial = cache.Get("tenant-hot");

    clock.Advance(TimeSpan.FromMinutes(4));
    source.SetVersion("v2");
    var minute4 = cache.Get("tenant-hot");

    clock.Advance(TimeSpan.FromMinutes(4));
    var minute8 = cache.Get("tenant-hot");

    clock.Advance(TimeSpan.FromMinutes(4));
    var minute12 = cache.Get("tenant-hot");
    var sourceAt12 = source.CurrentVersion;

    clock.Advance(TimeSpan.FromMinutes(1));
    var minute13 = cache.Get("tenant-hot");

    return new SimulationResult(initial, minute4, minute8, minute12, minute13, sourceAt12, source.ReadCount);
}

static PricingRuleCache BuildCache(FakeClock clock, PricingRuleSource source)
{
    // Investigation note:
    // Policy hiện tại được chọn để giữ các key hoạt động thường xuyên trong cache.
    // Kiểm tra chính xác điều gì xảy ra với freshness khi read traffic không bao giờ dừng.
    return new PricingRuleCache(clock, source, slidingWindow: TimeSpan.FromMinutes(5));
}

static void Print(SimulationResult r)
{
    Console.WriteLine($"t=00m cache={r.Initial}");
    Console.WriteLine($"t=04m cache={r.VersionAtMinute4} source=v2");
    Console.WriteLine($"t=08m cache={r.VersionAtMinute8} source=v2");
    Console.WriteLine($"t=12m cache={r.VersionAtMinute12} source={r.SourceVersionAtMinute12}");
    Console.WriteLine($"t=13m cache={r.VersionAtMinute13} source={r.SourceVersionAtMinute12}");
    Console.WriteLine($"sourceReads={r.SourceReads}");
}

sealed class PricingRuleCache
{
    private readonly FakeClock _clock;
    private readonly PricingRuleSource _source;
    private readonly TimeSpan _slidingWindow;
    private readonly Dictionary<string, CacheEntry> _entries = new();

    public PricingRuleCache(FakeClock clock, PricingRuleSource source, TimeSpan slidingWindow)
    {
        _clock = clock;
        _source = source;
        _slidingWindow = slidingWindow;
    }

    public string Get(string key)
    {
        if (_entries.TryGetValue(key, out var entry))
        {
            var idle = _clock.UtcNow - entry.LastAccessUtc;
            if (idle <= _slidingWindow)
            {
                entry.LastAccessUtc = _clock.UtcNow;
                return entry.Value;
            }
        }

        var value = _source.Read();
        _entries[key] = new CacheEntry(value, _clock.UtcNow, _clock.UtcNow);
        return value;
    }

    private sealed class CacheEntry
    {
        public CacheEntry(string value, DateTimeOffset createdUtc, DateTimeOffset lastAccessUtc)
        {
            Value = value;
            CreatedUtc = createdUtc;
            LastAccessUtc = lastAccessUtc;
        }

        public string Value { get; }
        public DateTimeOffset CreatedUtc { get; }
        public DateTimeOffset LastAccessUtc { get; set; }
    }
}

sealed class PricingRuleSource
{
    public PricingRuleSource(string initialVersion) => CurrentVersion = initialVersion;

    public string CurrentVersion { get; private set; }
    public int ReadCount { get; private set; }

    public void SetVersion(string version) => CurrentVersion = version;

    public string Read()
    {
        ReadCount++;
        return CurrentVersion;
    }
}

sealed class FakeClock
{
    public FakeClock(DateTimeOffset utcNow) => UtcNow = utcNow;
    public DateTimeOffset UtcNow { get; private set; }
    public void Advance(TimeSpan duration) => UtcNow += duration;
}

sealed record SimulationResult(
    string Initial,
    string VersionAtMinute4,
    string VersionAtMinute8,
    string VersionAtMinute12,
    string VersionAtMinute13,
    string SourceVersionAtMinute12,
    int SourceReads);
