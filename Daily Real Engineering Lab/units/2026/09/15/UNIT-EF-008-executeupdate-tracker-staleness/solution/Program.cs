using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

var options = new DbContextOptionsBuilder<CatalogDbContext>()
    .UseSqlite(connection)
    .Options;

await using var db = new CatalogDbContext(options);
await db.Database.EnsureCreatedAsync();

db.Products.Add(new Product { Sku = "SKU-100", Price = 100m });
await db.SaveChangesAsync();

var product = await db.Products.SingleAsync(p => p.Sku == "SKU-100");

await db.Products
    .Where(p => p.Sku == "SKU-100")
    .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.Price, p => p.Price + 10m));

await db.Entry(product).ReloadAsync();

var databasePrice = await db.Products
    .AsNoTracking()
    .Where(p => p.Sku == "SKU-100")
    .Select(p => p.Price)
    .SingleAsync();

var alertTriggered = product.Price >= 110m;

Console.WriteLine($"Price in database: {databasePrice:0.00}");
Console.WriteLine($"Price seen by rule: {product.Price:0.00}");
Console.WriteLine($"Alert triggered: {alertTriggered}");

return databasePrice == 110m && product.Price == 110m && alertTriggered ? 0 : 1;

public sealed class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
}

public sealed class Product
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
