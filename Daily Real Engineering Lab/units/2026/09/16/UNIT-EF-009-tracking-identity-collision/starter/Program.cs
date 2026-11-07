using Microsoft.EntityFrameworkCore;

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase("lab-db")
    .Options;

await using var db = new AppDbContext(options);
db.Products.Add(new Product { Id = 1, Name = "Keyboard", Price = 80m });
await db.SaveChangesAsync();

var current = await db.Products.SingleAsync(x => x.Id == 1);
Console.WriteLine($"Loaded: {current.Name} / {current.Price}");
Console.WriteLine($"Tracked entries before update: {db.ChangeTracker.Entries().Count()}");

var requestModel = new Product { Id = 1, Name = "Keyboard Pro", Price = 95m };

// Investigation point: the update path receives a separate object from the request.
// Inspect ChangeTracker state before deciding how this object should participate.
db.Products.Update(requestModel);
await db.SaveChangesAsync();

Console.WriteLine("Update completed");

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}

public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}