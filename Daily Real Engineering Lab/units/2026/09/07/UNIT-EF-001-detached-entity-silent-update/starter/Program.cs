using Microsoft.EntityFrameworkCore;

var databaseName = $"catalog-{Guid.NewGuid():N}";
var options = new DbContextOptionsBuilder<CatalogDbContext>()
    .UseInMemoryDatabase(databaseName)
    .Options;

await using (var seedDb = new CatalogDbContext(options))
{
    seedDb.Products.Add(new Product { Id = 1, Name = "Mechanical Keyboard", Price = 99m });
    await seedDb.SaveChangesAsync();
}

await using (var updateDb = new CatalogDbContext(options))
{
    var product = await updateDb.Products
        .AsNoTracking()
        .SingleAsync(x => x.Id == 1);

    product.Price = 129m;

    Console.WriteLine($"InMemoryPrice={product.Price:0}");
    Console.WriteLine($"ModifiedEntries={updateDb.ChangeTracker.Entries().Count(x => x.State == EntityState.Modified)}");

    var changedRows = await updateDb.SaveChangesAsync();
    Console.WriteLine($"ChangedRows={changedRows}");
}

await using (var readDb = new CatalogDbContext(options))
{
    var persistedPrice = await readDb.Products
        .AsNoTracking()
        .Where(x => x.Id == 1)
        .Select(x => x.Price)
        .SingleAsync();

    Console.WriteLine($"PersistedPrice={persistedPrice:0}");
}

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}

public sealed class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
}
