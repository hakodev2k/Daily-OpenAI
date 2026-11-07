using System.Globalization;

var clock = new FakeClock(DateTimeOffset.Parse("2026-09-10T00:00:00Z", CultureInfo.InvariantCulture));
var source = new PricingRuleSource("v1");
var cache = new PricingRuleCache(clock, source, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(10));

Console.WriteLine($"t=00m cache={cache.Get("tenant-hot")}");
clock.Advance(TimeSpan.FromMinutes(4));
source.SetVersion("v2");
Console.WriteLine($"t=04m cache={cache.Get("tenant-hot")} source=v2");
clock.Advance(TimeSpan.FromMinutes(4));
Console.WriteLine($"t=08m cache={cache.Get("tenant-hot")} source=v2");
clock.Advance(TimeSpan.FromMinutes(4));
Console.WriteLine($"t=12m cache={cache.Get("tenant-hot")} source=v2");
clock.Advance(TimeSpan.FromMinutes(1));
Console.WriteLine($"t=13m cache={cache.Get("tenant-hot")} source=v2");
Console.WriteLine($"sourceReads={source.ReadCount}");

sealed class PricingRuleCache
{
    private readonly FakeClock _clock;
    private readonly PricingRuleSource _source;
    private readonly TimeSpan _slidingWindow;
    private readonly TimeSpan _absoluteMaxAge;
    private readonly Dictionary<string, CacheEntry> _entries = new();

    public PricingRuleCache(FakeClock clock, PricingRuleSource source, TimeSpan slidingWindow, TimeSpan absoluteMaxAge)
    {
        _clock = clock;
        _source = source;
        _slidingWindow = slidingWindow;
        _absoluteMaxAge = absoluteMaxAge;
    }

    public string Get(string key)
    {
        if (_entries.TryGetValue(key, out var entry))
        {
            var idle = _clock.UtcNow - entry.LastAccessUtc;
            var age = _clock.UtcNow - entry.CreatedUtc;

            if (idle <= _slidingWindow && age <= _absoluteMaxAge)
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
    public string Read() { ReadCount++; return CurrentVersion; }
}

sealed class FakeClock
{
    public FakeClock(DateTimeOffset utcNow) => UtcNow = utcNow;
    public DateTimeOffset UtcNow { get; private set; }
    public void Advance(TimeSpan duration) => UtcNow += duration;
}
