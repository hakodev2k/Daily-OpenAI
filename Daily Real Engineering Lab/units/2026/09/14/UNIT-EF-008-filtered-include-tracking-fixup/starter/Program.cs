using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var mode = args.FirstOrDefault() ?? "reproduce";
await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

var options = new DbContextOptionsBuilder<SupportDbContext>()
    .UseSqlite(connection)
    .Options;

await using var db = new SupportDbContext(options);
await db.Database.EnsureCreatedAsync();
await SeedAsync(db);

// Earlier work in the same request: support audit needs full order history.
var auditHistory = await db.Orders
    .Where(o => o.CustomerId == 1)
    .OrderBy(o => o.Id)
    .ToListAsync();

Console.WriteLine($"Audit query loaded {auditHistory.Count} orders.");

// Endpoint contract: customer with OPEN orders only.
var customer = await db.Customers
    .Include(c => c.Orders.Where(o => o.Status == OrderStatus.Open))
    .SingleAsync(c => c.Id == 1);

var statuses = string.Join(", ", customer.Orders.OrderBy(o => o.Id).Select(o => o.Status));
Console.WriteLine($"Endpoint Orders: {statuses}");

var contractIsCorrect = customer.Orders.All(o => o.Status == OrderStatus.Open);

if (mode.Equals("verify", StringComparison.OrdinalIgnoreCase))
{
    if (!contractIsCorrect)
    {
        Console.Error.WriteLine("VERIFY FAILED: endpoint object graph contains an order outside the requested status.");
        Environment.ExitCode = 1;
        return;
    }

    Console.WriteLine("VERIFY PASSED: endpoint object graph contains open orders only.");
    return;
}

if (contractIsCorrect)
{
    Console.Error.WriteLine("REPRODUCE FAILED: intended symptom did not occur.");
    Environment.ExitCode = 1;
    return;
}

Console.WriteLine("REPRODUCE PASSED: the endpoint object graph contains data outside its intended filter.");

static async Task SeedAsync(SupportDbContext db)
{
    if (await db.Customers.AnyAsync()) return;

    var customer = new Customer { Id = 1, Name = "Contoso Retail" };
    customer.Orders.Add(new Order { Id = 101, Status = OrderStatus.Open });
    customer.Orders.Add(new Order { Id = 102, Status = OrderStatus.Closed });
    customer.Orders.Add(new Order { Id = 103, Status = OrderStatus.Open });
    db.Customers.Add(customer);
    await db.SaveChangesAsync();
    db.ChangeTracker.Clear();
}

sealed class SupportDbContext(DbContextOptions<SupportDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId);
    }
}

sealed class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Order> Orders { get; } = [];
}

sealed class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public OrderStatus Status { get; set; }
}

enum OrderStatus
{
    Open = 1,
    Closed = 2
}
