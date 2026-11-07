using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connection)
    .Options;

await using var db = new AppDbContext(options);
await db.Database.EnsureCreatedAsync();

db.Orders.Add(new Order { Id = 1, Status = "Pending" });
await db.SaveChangesAsync();

var order = await db.Orders.SingleAsync(x => x.Id == 1);
Console.WriteLine($"BEFORE={order.Status}");

var affected = await db.Orders
    .Where(x => x.Id == 1)
    .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Status, "Approved"));

await db.Entry(order).ReloadAsync();

Console.WriteLine($"AFFECTED={affected}");
Console.WriteLine($"TRACKED={order.Status}");

var databaseValue = await db.Orders
    .AsNoTracking()
    .Where(x => x.Id == 1)
    .Select(x => x.Status)
    .SingleAsync();

Console.WriteLine($"DATABASE={databaseValue}");

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
}

public sealed class Order
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
}
