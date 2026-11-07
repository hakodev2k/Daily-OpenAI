using Microsoft.EntityFrameworkCore;

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase("orders")
    .Options;

await SeedAsync(options);

await using var db = new AppDbContext(options);

// Mô phỏng một bước xử lý trước đó trong cùng request.
var preloaded = await db.Orders
    .Include(x => x.Lines)
    .SingleAsync(x => x.Id == 1);

Console.WriteLine($"preloadedLines={preloaded.Lines.Count}");

// Contract của bước này: chỉ trả về line đang Open.
var order = await db.Orders
    .Include(x => x.Lines.Where(line => line.Status == "Open"))
    .SingleAsync(x => x.Id == 1);

var openInStore = order.Lines.Count(x => x.Status == "Open");
Console.WriteLine($"openInStore={openInStore}");
Console.WriteLine($"returnedLines={order.Lines.Count}");
Console.WriteLine("statuses=" + string.Join(',', order.Lines.Select(x => x.Status)));

static async Task SeedAsync(DbContextOptions<AppDbContext> options)
{
    await using var db = new AppDbContext(options);
    if (await db.Orders.AnyAsync()) return;

    db.Orders.Add(new Order
    {
        Id = 1,
        Lines =
        [
            new OrderLine { Id = 11, Status = "Open" },
            new OrderLine { Id = 12, Status = "Closed" },
            new OrderLine { Id = 13, Status = "Open" }
        ]
    });

    await db.SaveChangesAsync();
}

sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
}

sealed class Order
{
    public int Id { get; set; }
    public List<OrderLine> Lines { get; set; } = [];
}

sealed class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Status { get; set; } = "";
}
