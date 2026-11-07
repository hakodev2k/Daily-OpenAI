using Microsoft.Extensions.Caching.Memory;

using var cache = new MemoryCache(new MemoryCacheOptions());
var service = new PricingService(cache);

var discounted = service.GetPrice(productId: 42, applyPromotion: true);
var normal = service.GetPrice(productId: 42, applyPromotion: false);
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
            _ => new ProductPrice { ProductId = productId, Price = 100m })!;

        if (applyPromotion)
        {
            price.Price *= 0.8m;
        }

        return price.Price;
    }
}

public sealed class ProductPrice
{
    public int ProductId { get; init; }
    public decimal Price { get; set; }
}
