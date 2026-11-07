using System.Collections.Concurrent;
using SolarTariffLab;

namespace SolarTariffLab.Starter;

public sealed class TariffCache(ITariffProvider provider, ManualClock clock) : ITariffCache
{
    private sealed record Entry(TariffQuote Quote, DateTimeOffset ExpiresAt);
    private readonly ConcurrentDictionary<string, Entry> _entries =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    public async Task<TariffQuote> GetAsync(string region)
    {
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region is required", nameof(region));

        if (_entries.TryGetValue(region, out var entry) && entry.ExpiresAt > clock.UtcNow)
            return entry.Quote;

        var quote = await provider.FetchAsync(region).ConfigureAwait(false);
        _entries[region] = new Entry(quote, clock.UtcNow.Add(Ttl));
        return quote;
    }
}
