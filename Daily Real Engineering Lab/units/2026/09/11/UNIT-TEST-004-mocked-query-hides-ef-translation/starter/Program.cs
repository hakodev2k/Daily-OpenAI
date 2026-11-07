using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var mode = args.FirstOrDefault() ?? "compare";
var products = Seed.Products();

if (mode is "memory" or "compare")
{
    var result = ProductSearch.Search(products.AsQueryable(), "  ACME   WIDGET ").ToList();
    Console.WriteLine($"memory-count={result.Count}");
}

if (mode is "sqlite" or "compare")
{
    using var connection = new SqliteConnection("Data Source=:memory:");
    connection.Open();
    var options = new DbContextOptionsBuilder<ProductDb>().UseSqlite(connection).Options;
    using var db = new ProductDb(options);
    db.Database.EnsureCreated();
    db.Products.AddRange(products);
    db.SaveChanges();

    try
    {
        var result = ProductSearch.Search(db.Products.AsNoTracking(), "  ACME   WIDGET ").ToList();
        Console.WriteLine($"sqlite-count={result.Count}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"sqlite-error={ex.GetType().Name}");
        Environment.ExitCode = 2;
    }
}

public static class ProductSearch
{
    public static IQueryable<Product> Search(IQueryable<Product> source, string term)
    {
        // Investigation note: this helper originally came from application-side normalization code.
        return source.Where(p => Normalize(p.Name).Contains(Normalize(term)));
    }

    private static string Normalize(string value) =>
        string.Join(' ', value.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
}

public sealed class ProductDb(DbContextOptions<ProductDb> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}

public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public static class Seed
{
    public static List<Product> Products() =>
    [
        new() { Id = 1, Name = "Acme Widget" },
        new() { Id = 2, Name = "Acme Cable" },
        new() { Id = 3, Name = "Contoso Widget" }
    ];
}