using System.Collections.Concurrent;

const int concurrentRequests = 24;
const string hotKey = "sku-42";

var cache = new ExpiringCache<Product>();
var store = new CatalogStore();
var service = new CatalogService(cache, store);

cache.Set(hotKey, new Product(42, "Flash-sale keyboard", 0), TimeSpan.FromMilliseconds(-1));

var startGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
var requests = Enumerable.Range(0, concurrentRequests)
    .Select(async _ =>
    {
        await startGate.Task;
        return await service.GetAsync(hotKey);
    })
    .ToArray();

startGate.SetResult(true);
var results = await Task.WhenAll(requests);

var first = results[0];
var consistent = results.All(x => x.Id == first.Id && x.Name == first.Name && x.Version == first.Version);

Console.WriteLine($"REQUESTS={concurrentRequests}");
Console.WriteLine($"DOWNSTREAM_CALLS={store.CallCount}");
Console.WriteLine($"RESULTS_CONSISTENT={consistent}");
Console.WriteLine($"PRODUCT_VERSION={first.Version}");

internal sealed record Product(int Id, string Name, int Version);

internal sealed class ExpiringCache<T>
{
    private readonly ConcurrentDictionary<string, Entry> _entries = new();

    public bool TryGet(string key, out T value)
    {
        if (_entries.TryGetValue(key, out var entry) && entry.ExpiresAt > DateTimeOffset.UtcNow)
        {
            value = entry.Value;
            return true;
        }

        value = default!;
        return false;
    }

    public void Set(string key, T value, TimeSpan ttl)
        => _entries[key] = new Entry(value, DateTimeOffset.UtcNow.Add(ttl));

    private sealed record Entry(T Value, DateTimeOffset ExpiresAt);
}

internal sealed class CatalogStore
{
    private int _callCount;

    public int CallCount => Volatile.Read(ref _callCount);

    public async Task<Product> LoadAsync(string key)
    {
        Interlocked.Increment(ref _callCount);
        await Task.Delay(150);
        return new Product(42, "Flash-sale keyboard", 7);
    }
}

internal sealed class CatalogService
{
    private readonly ExpiringCache<Product> _cache;
    private readonly CatalogStore _store;

    public CatalogService(ExpiringCache<Product> cache, CatalogStore store)
    {
        _cache = cache;
        _store = store;
    }

    public async Task<Product> GetAsync(string key)
    {
        if (_cache.TryGet(key, out var cached))
        {
            return cached;
        }

        // Investigation note:
        // Khi nhiều request cùng quan sát trạng thái này, có bao nhiêu request
        // thực sự cần thực hiện expensive dependency work?
        var loaded = await _store.LoadAsync(key);
        _cache.Set(key, loaded, TimeSpan.FromMinutes(1));
        return loaded;
    }
}
