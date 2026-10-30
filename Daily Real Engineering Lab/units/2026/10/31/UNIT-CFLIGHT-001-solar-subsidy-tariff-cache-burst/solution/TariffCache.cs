using System.Collections;
using System.Collections.Concurrent;
using System.Threading;
using SolarTariffLab;

namespace SolarTariffLab.Solution;

public sealed class TariffCache(ITariffProvider provider, ManualClock clock) : ITariffCache
{
    private sealed record Entry(TariffQuote Quote, DateTimeOffset ExpiresAt);
    private readonly ConcurrentDictionary<string, Entry> _entries =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, Lazy<Task<TariffQuote>>> _inflight =
        new(StringComparer.OrdinalIgnoreCase);
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    public async Task<TariffQuote> GetAsync(string region)
    {
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region is required", nameof(region));

        if (_entries.TryGetValue(region, out var entry) && entry.ExpiresAt > clock.UtcNow)
            return entry.Quote;

        // Lazy bảo đảm chỉ factory của giá trị thắng GetOrAdd mới gọi upstream.
        var flight = _inflight.GetOrAdd(region, key =>
            new Lazy<Task<TariffQuote>>(
                () => FetchAndStoreAsync(key),
                LazyThreadSafetyMode.ExecutionAndPublication));

        try
        {
            return await flight.Value.ConfigureAwait(false);
        }
        finally
        {
            // Chỉ gỡ đúng flight đã chờ; không xóa flight mới của request khác.
            ((ICollection<KeyValuePair<string, Lazy<Task<TariffQuote>>>>)_inflight)
                .Remove(new KeyValuePair<string, Lazy<Task<TariffQuote>>>(region, flight));
        }
    }

    private async Task<TariffQuote> FetchAndStoreAsync(string region)
    {
        var quote = await provider.FetchAsync(region).ConfigureAwait(false);
        _entries[region] = new Entry(quote, clock.UtcNow.Add(Ttl));
        return quote;
    }
}
