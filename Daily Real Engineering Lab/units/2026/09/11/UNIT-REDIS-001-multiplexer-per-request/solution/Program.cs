using System.Diagnostics;

var metrics = new Metrics();
var sharedClient = await FakeRedisClient.ConnectAsync(metrics);
var service = new PricingCacheService(sharedClient);

var sw = Stopwatch.StartNew();
var tasks = Enumerable.Range(0, 24).Select(_ => service.GetPriceAsync("sku-42")).ToArray();
var results = await Task.WhenAll(tasks);
sw.Stop();

Console.WriteLine($"results={results.Length}");
Console.WriteLine($"distinctPrices={results.Distinct().Count()}");
Console.WriteLine($"clientsCreated={metrics.ClientsCreated}");
Console.WriteLine($"handshakes={metrics.Handshakes}");
Console.WriteLine($"elapsedMs={sw.ElapsedMilliseconds}");

public sealed class PricingCacheService
{
    private readonly FakeRedisClient _client;
    public PricingCacheService(FakeRedisClient client) => _client = client;
    public Task<decimal> GetPriceAsync(string sku) => _client.GetPriceAsync(sku);
}

public sealed class FakeRedisClient
{
    private readonly Metrics _metrics;
    private FakeRedisClient(Metrics metrics) => _metrics = metrics;

    public static async Task<FakeRedisClient> ConnectAsync(Metrics metrics)
    {
        Interlocked.Increment(ref metrics.ClientsCreated);
        Interlocked.Increment(ref metrics.Handshakes);
        await Task.Delay(80);
        return new FakeRedisClient(metrics);
    }

    public async Task<decimal> GetPriceAsync(string sku)
    {
        await Task.Delay(5);
        return sku == "sku-42" ? 19.99m : 0m;
    }
}

public sealed class Metrics
{
    public int ClientsCreated;
    public int Handshakes;
}
