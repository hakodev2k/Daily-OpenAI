using System.Collections.Concurrent;
using System.Threading;

namespace SolarTariffLab;

public sealed record TariffQuote(string Region, decimal AmountVnd);
public interface ITariffProvider
{
    Task<TariffQuote> FetchAsync(string region);
}
public interface ITariffCache
{
    Task<TariffQuote> GetAsync(string region);
}

public sealed class ManualClock(DateTimeOffset initial)
{
    private DateTimeOffset _now = initial;
    public DateTimeOffset UtcNow => _now;
    public void Advance(TimeSpan duration) => _now = _now.Add(duration);
}

public sealed class CountingTariffProvider : ITariffProvider
{
    private readonly ConcurrentDictionary<string, byte> _failNext = new(StringComparer.OrdinalIgnoreCase);
    private int _calls;
    private int _active;
    private int _maxActive;
    public int CallCount => Volatile.Read(ref _calls);
    public int MaxActive => Volatile.Read(ref _maxActive);

    public void FailNext(string region) => _failNext[region] = 1;

    public async Task<TariffQuote> FetchAsync(string region)
    {
        Interlocked.Increment(ref _calls);
        int active = Interlocked.Increment(ref _active);
        int observed;
        do
        {
            observed = Volatile.Read(ref _maxActive);
            if (active <= observed) break;
        } while (Interlocked.CompareExchange(ref _maxActive, active, observed) != observed);

        try
        {
            // Giả lập độ trễ cố định của nhà cung cấp; không cần network hay Redis.
            await Task.Delay(200).ConfigureAwait(false);
            if (_failNext.TryRemove(region, out _))
                throw new InvalidOperationException("Partner tariff feed temporarily unavailable");
            return new TariffQuote(region, 125_000m);
        }
        finally
        {
            Interlocked.Decrement(ref _active);
        }
    }
}

public sealed record ScenarioResult(
    bool WarmCacheHit,
    int BurstProviderCalls,
    bool AllQuotesAgree,
    bool IndependentKeysOverlap,
    bool FailedFetchCanRetry)
{
    public bool CommonContract =>
        WarmCacheHit && AllQuotesAgree && IndependentKeysOverlap && FailedFetchCanRetry;

    public string Report() =>
        $"WarmCacheHit={WarmCacheHit}; BurstProviderCalls={BurstProviderCalls}; " +
        $"AllQuotesAgree={AllQuotesAgree}; IndependentKeysOverlap={IndependentKeysOverlap}; " +
        $"FailedFetchCanRetry={FailedFetchCanRetry}";
}

public static class ScenarioRunner
{
    public static async Task<ScenarioResult> RunAsync(
        Func<ITariffProvider, ManualClock, ITariffCache> createCache)
    {
        var clock = new ManualClock(new DateTimeOffset(2026, 10, 31, 0, 0, 0, TimeSpan.Zero));
        var provider = new CountingTariffProvider();
        var cache = createCache(provider, clock);

        var first = await cache.GetAsync("north");
        var second = await cache.GetAsync("north");
        bool warmHit = provider.CallCount == 1 && first == second;

        // Chỉ thay đổi thời gian giả lập trước burst; không có sleep cho TTL.
        clock.Advance(TimeSpan.FromMinutes(6));
        const int clients = 12;
        int waiting = 0;
        var ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var tasks = Enumerable.Range(0, clients).Select(_ => Task.Run(async () =>
        {
            if (Interlocked.Increment(ref waiting) == clients) ready.TrySetResult();
            await start.Task.ConfigureAwait(false);
            return await cache.GetAsync("north").ConfigureAwait(false);
        })).ToArray();

        await ready.Task.WaitAsync(TimeSpan.FromSeconds(10));
        start.TrySetResult();
        var quotes = await Task.WhenAll(tasks);
        int burstCalls = provider.CallCount - 1;
        bool agree = quotes.All(q => q == first);

        // Hai key khác nhau phải được phép đi tới upstream song song.
        var parallelProvider = new CountingTariffProvider();
        var parallelCache = createCache(parallelProvider, clock);
        await Task.WhenAll(parallelCache.GetAsync("west"), parallelCache.GetAsync("east"));
        bool independent = parallelProvider.MaxActive >= 2;

        // Lỗi tạm thời không được làm cache hoặc inflight entry bị kẹt vĩnh viễn.
        var retryProvider = new CountingTariffProvider();
        retryProvider.FailNext("south");
        var retryCache = createCache(retryProvider, clock);
        bool sawFailure = false;
        try { await retryCache.GetAsync("south"); }
        catch (InvalidOperationException) { sawFailure = true; }
        var recovered = await retryCache.GetAsync("south");
        bool retryOk = sawFailure && recovered.Region == "south" &&
                       retryProvider.CallCount == 2;

        return new ScenarioResult(warmHit, burstCalls, agree, independent, retryOk);
    }
}
