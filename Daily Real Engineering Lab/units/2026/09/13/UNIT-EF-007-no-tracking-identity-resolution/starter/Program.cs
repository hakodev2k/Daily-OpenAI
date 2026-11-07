using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

var options = new DbContextOptionsBuilder<LabDbContext>()
    .UseSqlite(connection)
    .Options;

await using (var setup = new LabDbContext(options))
{
    await setup.Database.EnsureCreatedAsync();

    var customer = new Customer { Id = 42, Name = "Contoso Logistics" };
    setup.Customers.Add(customer);
    setup.Orders.AddRange(
        new Order { Id = 1001, CustomerId = 42, Reference = "SHIP-A" },
        new Order { Id = 1002, CustomerId = 42, Reference = "SHIP-B" },
        new Order { Id = 1003, CustomerId = 42, Reference = "SHIP-C" });

    await setup.SaveChangesAsync();
}

await using var db = new LabDbContext(options);

// Investigation note:
// This is intentionally a read-only query. Observe both database identity and CLR object identity.
var orders = await db.Orders
    .Include(x => x.Customer)
    .AsNoTracking()
    .OrderBy(x => x.Id)
    .ToListAsync();

var customerIds = orders.Select(x => x.CustomerId).Distinct().Count();
var customerObjectCount = orders
    .Select(x => x.Customer)
    .Distinct(ReferenceEqualityComparer.Instance)
    .Count();

Console.WriteLine($"Orders={orders.Count}");
Console.WriteLine($"DistinctCustomerIds={customerIds}");
Console.WriteLine($"DistinctCustomerObjects={customerObjectCount}");

if (orders.Count != 3 || customerIds != 1)
{
    Console.Error.WriteLine("Functional data is not as expected.");
    return 2;
}

if (customerObjectCount != 1)
{
    Console.Error.WriteLine("Read model contains multiple CLR instances for one database customer.");
    return 1;
}

Console.WriteLine("PASS: one CLR customer instance represents the shared database customer.");
return 0;

sealed class LabDbContext(DbContextOptions<LabDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
}

sealed class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Order> Orders { get; set; } = [];
}

sealed class Order
{
    public int Id { get; set; }
    public required string Reference { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}
