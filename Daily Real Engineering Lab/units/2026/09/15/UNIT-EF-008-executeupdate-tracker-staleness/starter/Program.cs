using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var mode = args.FirstOrDefault() ?? "--reproduce";

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
var priceBefore = product.Price;

await db.Products
    .Where(p => p.Sku == "SKU-100")
    .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.Price, p => p.Price + 10m));

var databasePrice = await db.Products
    .AsNoTracking()
    .Where(p => p.Sku == "SKU-100")
    .Select(p => p.Price)
    .SingleAsync();

var priceSeenByBusinessRule = product.Price;
var alertTriggered = priceSeenByBusinessRule >= 110m;

Console.WriteLine($"Price before repricing: {priceBefore:0.00}");
Console.WriteLine($"Price in database: {databasePrice:0.00}");
Console.WriteLine($"Price seen by rule: {priceSeenByBusinessRule:0.00}");
Console.WriteLine($"Alert triggered: {alertTriggered}");

if (mode == "--reproduce")
{
    var reproduced = databasePrice == 110m && priceSeenByBusinessRule == 100m && !alertTriggered;
    Console.WriteLine(reproduced ? "REPRODUCED" : "REPRODUCE FAILED");
    return reproduced ? 0 : 1;
}

if (mode == "--verify")
{
    var verified = databasePrice == 110m && priceSeenByBusinessRule == 110m && alertTriggered;
    Console.WriteLine(verified ? "VERIFIED" : "VERIFY FAILED");
    return verified ? 0 : 1;
}

return 2;

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
