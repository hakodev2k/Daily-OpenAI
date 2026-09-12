using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var mode = args.FirstOrDefault()?.ToLowerInvariant() ?? "observe";

await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connection)
    .AddInterceptors(new SlowReaderInterceptor(TimeSpan.FromMilliseconds(250)))
    .Options;

await using var db = new AppDbContext(options);
await db.Database.EnsureCreatedAsync();

if (!await db.Orders.AnyAsync())
{
    db.Orders.AddRange(
        new Order { Number = "ORD-1001", Total = 120 },
        new Order { Number = "ORD-1002", Total = 80 });
    db.Alerts.AddRange(
        new Alert { Message = "Payment review" },
        new Alert { Message = "Stock warning" });
    await db.SaveChangesAsync();
}

var service = new DashboardService(db);

try
{
    var result = await service.LoadAsync();
    Console.WriteLine($"Orders={result.OrderCount}; Alerts={result.AlertCount}");

    if (mode == "reproduce")
    {
        Console.Error.WriteLine("REPRODUCE FAILED: expected overlapping-operation failure was not observed.");
        return 2;
    }

    if (result.OrderCount != 2 || result.AlertCount != 2)
    {
        Console.Error.WriteLine("VERIFY FAILED: business result changed.");
        return 3;
    }

    Console.WriteLine("VERIFY PASSED: dashboard returned both result sets without concurrent-operation failure.");
    return 0;
}
catch (InvalidOperationException ex) when (ex.Message.Contains("second operation", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("Observed expected EF Core concurrent-operation exception:");
    Console.WriteLine(ex.Message);

    if (mode == "reproduce")
    {
        Console.WriteLine("REPRODUCE PASSED: intended failure was observed.");
        return 0;
    }

    Console.Error.WriteLine("VERIFY FAILED: learner-editable starter still overlaps operations on one context.");
    return 4;
}

public sealed class DashboardService(AppDbContext db)
{
    public async Task<DashboardResult> LoadAsync()
    {
        // Investigation note: both operations look independently async-safe.
        // What shared execution boundary exists between them?
        var ordersTask = db.Orders.AsNoTracking().ToListAsync();
        var alertsTask = db.Alerts.AsNoTracking().ToListAsync();

        await Task.WhenAll(ordersTask, alertsTask);
        return new DashboardResult(ordersTask.Result.Count, alertsTask.Result.Count);
    }
}

public sealed record DashboardResult(int OrderCount, int AlertCount);

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Alert> Alerts => Set<Alert>();
}

public sealed class Order
{
    public int Id { get; set; }
    public string Number { get; set; } = "";
    public decimal Total { get; set; }
}

public sealed class Alert
{
    public int Id { get; set; }
    public string Message { get; set; } = "";
}

public sealed class SlowReaderInterceptor(TimeSpan delay) : DbCommandInterceptor
{
    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(delay, cancellationToken);
        return result;
    }
}
