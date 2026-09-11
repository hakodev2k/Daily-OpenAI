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

    var result = ProductSearch.Search(db.Products.AsNoTracking(), "  ACME   WIDGET ").ToList();
    Console.WriteLine($"sqlite-count={result.Count}");
}

public static class ProductSearch
{
    public static IQueryable<Product> Search(IQueryable<Product> source, string term)
    {
        var normalizedTerm = NormalizeTerm(term);
        return source.Where(p => p.Name.Trim().ToLower().Contains(normalizedTerm));
    }

    private static string NormalizeTerm(string value) =>
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