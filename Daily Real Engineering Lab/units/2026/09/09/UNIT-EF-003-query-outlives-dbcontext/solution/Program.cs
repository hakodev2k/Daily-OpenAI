using Microsoft.EntityFrameworkCore;

var repository = new ProductRepository();
var products = await repository.GetLowStockAsync(5);

Console.WriteLine($"RESULT_COUNT={products.Count}");
foreach (var product in products)
{
    Console.WriteLine($"{product.Sku}:{product.Stock}");
}

return 0;

sealed class ProductRepository
{
    public async Task<IReadOnlyList<ProductSummary>> GetLowStockAsync(int threshold)
    {
        await using var db = CreateContext();

        return await db.Products
            .Where(product => product.Stock <= threshold)
            .OrderBy(product => product.Sku)
            .Select(product => new ProductSummary(product.Sku, product.Stock))
            .ToListAsync();
    }

    private static InventoryDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase($"inventory-{Guid.NewGuid():N}")
            .Options;

        var db = new InventoryDbContext(options);
        db.Products.AddRange(
            new Product { Id = 1, Sku = "SKU-LOW-001", Stock = 2 },
            new Product { Id = 2, Sku = "SKU-LOW-002", Stock = 5 },
            new Product { Id = 3, Sku = "SKU-OK-003", Stock = 18 });
        db.SaveChanges();
        return db;
    }
}

sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}

sealed class Product
{
    public int Id { get; set; }
    public required string Sku { get; set; }
    public int Stock { get; set; }
}

sealed record ProductSummary(string Sku, int Stock);
