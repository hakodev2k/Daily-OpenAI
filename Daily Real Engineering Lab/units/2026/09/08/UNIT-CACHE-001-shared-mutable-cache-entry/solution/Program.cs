using Microsoft.Extensions.Caching.Memory;

using var cache = new MemoryCache(new MemoryCacheOptions());
var service = new PricingService(cache);

var discounted = service.GetPrice(42, true);
var normal = service.GetPrice(42, false);
var cached = cache.Get<ProductPrice>("product-price:42");

Console.WriteLine($"DiscountedResponse={discounted:0}");
Console.WriteLine($"NormalResponse={normal:0}");
Console.WriteLine($"CachedBasePrice={cached?.Price:0}");

public sealed class PricingService(IMemoryCache cache)
{
    public decimal GetPrice(int productId, bool applyPromotion)
    {
        var price = cache.GetOrCreate(
            $"product-price:{productId}",
            _ => new ProductPrice(productId, 100m))!;

        return applyPromotion ? price.Price * 0.8m : price.Price;
    }
}

public sealed record ProductPrice(int ProductId, decimal Price);
