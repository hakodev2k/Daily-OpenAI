using System.Collections.Concurrent;

const int requestCount = 20;
const string key = "product-42";

var cache = new SimpleCache();
var source = new PricingSource();
var service = new PricingService(cache, source);

var prices = await Task.WhenAll(
    Enumerable.Range(0, requestCount).Select(_ => service.GetPriceAsync(key)));

Console.WriteLine($"requestCount={requestCount}");
Console.WriteLine($"sourceCalls={source.Calls}");
Console.WriteLine($"allPricesCorrect={prices.All(p => p == 125_000m)}");

sealed class PricingService(SimpleCache cache, PricingSource source)
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _gates = new();

    public async Task<decimal> GetPriceAsync(string key)
    {
        if (cache.TryGet(key, out var cached))
        {
            return cached;
        }

        var gate = _gates.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync();
        try
        {
            if (cache.TryGet(key, out cached))
            {
                return cached;
            }

            var price = await source.LoadAsync(key);
            cache.Set(key, price);
            return price;
        }
        finally
        {
            gate.Release();
        }
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
