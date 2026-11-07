var catalog = new FakeCatalog();
var skus = new[] { "A", "B", "C", "D" };

var lines = skus.Select(sku => new InvoiceLine(sku, catalog.GetPrice(sku)));

var total = lines.Sum(x => x.Price);
var expensiveCount = lines.Count(x => x.Price >= 30m);

Console.WriteLine($"Total={total}");
Console.WriteLine($"Expensive={expensiveCount}");
Console.WriteLine($"CatalogCalls={catalog.CallCount}");

public sealed record InvoiceLine(string Sku, decimal Price);

public sealed class FakeCatalog
{
    private readonly Dictionary<string, decimal> _prices = new()
    {
        ["A"] = 10m, ["B"] = 20m, ["C"] = 30m, ["D"] = 40m
    };

    public int CallCount { get; private set; }

    public decimal GetPrice(string sku)
    {
        CallCount++;
        return _prices[sku];
    }
}
