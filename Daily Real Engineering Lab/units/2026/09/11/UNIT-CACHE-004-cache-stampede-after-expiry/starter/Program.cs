using System.Collections.Concurrent;

const int requestCount = 20;
const string key = "product-42";

var cache = new SimpleCache();
var source = new PricingSource();
var service = new PricingService(cache, source);

var tasks = Enumerable.Range(0, requestCount)
    .Select(_ => service.GetPriceAsync(key))
    .ToArray();

var prices = await Task.WhenAll(tasks);
var allCorrect = prices.All(price => price == 125_000m);

Console.WriteLine($"requestCount={requestCount}");
Console.WriteLine($"sourceCalls={source.Calls}");
Console.WriteLine($"allPricesCorrect={allCorrect}");

if (args.Contains("--verify", StringComparer.OrdinalIgnoreCase))
{
    return allCorrect && source.Calls == 1 ? 0 : 1;
}

return allCorrect && source.Calls > 1 ? 0 : 1;

sealed class PricingService(SimpleCache cache, PricingSource source)
{
    public async Task<decimal> GetPriceAsync(string key)
    {
        if (cache.TryGet(key, out var cached))
        {
            return cached;
        }

        // Investigation note:
        // Under concurrent misses, observe how many callers reach the source.
        var price = await source.LoadAsync(key);
        cache.Set(key, price);
        return price;
    }
}

sealed class SimpleCache
{
    private readonly ConcurrentDictionary<string, decimal> _values = new();

    public bool TryGet(string key, out decimal value) => _values.TryGetValue(key, out value);

    public void Set(string key, decimal value) => _values[key] = value;
}

sealed class PricingSource
{
    private int _calls;
    public int Calls => Volatile.Read(ref _calls);

    public async Task<decimal> LoadAsync(string key)
    {
        Interlocked.Increment(ref _calls);
        await Task.Delay(150);
        return key == "product-42" ? 125_000m : throw new InvalidOperationException("Unknown product");
    }
}
