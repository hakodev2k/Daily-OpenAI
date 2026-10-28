using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using StreetlightLab;

using var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();

var options = new DbContextOptionsBuilder<WorkOrdersContext>()
    .UseSqlite(connection)
    .Options;

// Một pool nhỏ giúp giữ thứ tự tái sử dụng context ổn định trong fixture.
var factory = new PooledDbContextFactory<WorkOrdersContext>(options, poolSize: 1);

using (var seed = factory.CreateDbContext())
{
    seed.Database.EnsureCreated();
    seed.WorkOrders.AddRange(
        new WorkOrder { Id = 1, TenantId = "north", Code = "N-101", IsOpen = true },
        new WorkOrder { Id = 2, TenantId = "north", Code = "N-102", IsOpen = false },
        new WorkOrder { Id = 3, TenantId = "south", Code = "S-202", IsOpen = true });
    seed.SaveChanges();
}

var reader = new WorkOrderReader(factory);
var cases = new (string Tenant, string[] Expected)[]
{
    ("north", new[] { "N-101" }),
    ("south", new[] { "S-202" }),
    ("new-tenant", Array.Empty<string>()),
    ("north", new[] { "N-101" })
};

var failures = 0;
foreach (var (tenant, expected) in cases)
{
    var actual = reader.GetOpenCodes(tenant);
    var ok = actual.SequenceEqual(expected, StringComparer.Ordinal);
    Console.WriteLine(
        $"{(ok ? "PASS" : "FAIL")} tenant={tenant} expected=[{string.Join(",", expected)}] actual=[{string.Join(",", actual)}]");
    if (!ok) failures++;
}

Console.WriteLine($"ISOLATION_CHECK failures={failures} total={cases.Length}");
return failures == 0 ? 0 : 17;
