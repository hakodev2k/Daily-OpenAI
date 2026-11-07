using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WastewaterLab;

var mode = args.Length == 1 ? args[0].ToLowerInvariant() : "";
if (mode is not ("observe" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project checks/Checks.csproj -- observe|reproduce|verify");
    return 2;
}

var file = Path.Combine(Path.GetTempPath(), $"efctx-lab-{Guid.NewGuid():N}.db");
var connectionString = new SqliteConnectionStringBuilder
{
    DataSource = file,
    Mode = SqliteOpenMode.ReadWriteCreate
}.ToString();

try
{
    var seedOptions = new DbContextOptionsBuilder<InspectionDbContext>()
        .UseSqlite(connectionString).Options;

    await using (var seed = new InspectionDbContext(seedOptions))
    {
        await seed.Database.EnsureCreatedAsync();
        seed.Inspections.AddRange(
            new Inspection { Id = 1, TicketId = "WW-101", Status = "Open" },
            new Inspection { Id = 2, TicketId = "WW-102", Status = "Closed" },
            new Inspection { Id = 3, TicketId = "WW-103", Status = "Open" });
        await seed.SaveChangesAsync();
    }

    var gate = new FirstQueryGate();
    var queryOptions = new DbContextOptionsBuilder<InspectionDbContext>()
        .UseSqlite(connectionString)
        .AddInterceptors(gate)
        .Options;

    var service = new InspectionReportService(() => new InspectionDbContext(queryOptions));
    var work = service.BuildAsync();

    // Hold the first query in the I/O pipeline until the service has launched
    // its other query. This makes the test independent of machine speed.
    try
    {
        await gate.Entered.WaitAsync(TimeSpan.FromSeconds(10));
    }
    finally
    {
        gate.Release();
    }

    try
    {
        var report = await work;
        Console.WriteLine($"Open: {string.Join(",", report.OpenTickets)}");
        Console.WriteLine($"Closed: {string.Join(",", report.ClosedTickets)}");
        bool correct = report.OpenTickets.SequenceEqual(new[] { "WW-101", "WW-103" })
                    && report.ClosedTickets.SequenceEqual(new[] { "WW-102" });
        if (!correct)
        {
            Console.Error.WriteLine("FAIL: report contains missing, duplicated, or misclassified tickets.");
            return 1;
        }

        if (mode == "reproduce")
        {
            Console.Error.WriteLine("FAIL: expected original starter symptom was not observed.");
            return 1;
        }

        Console.WriteLine("PASS: both report sections are complete under forced overlap.");
        return 0;
    }
    catch (InvalidOperationException ex)
        when (ex.Message.Contains("A second operation was started", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine($"Observed exception: {ex.GetType().Name}: {ex.Message}");
        if (mode is "reproduce" or "observe")
        {
            Console.WriteLine("PASS: expected original starter symptom reproduced.");
            return 0;
        }

        Console.Error.WriteLine("FAIL: verification detected the original concurrency symptom.");
        return 1;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"FAIL: unexpected exception: {ex}");
        return 1;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"FAIL: setup or synchronization error: {ex}");
    return 1;
}
finally
{
    try { File.Delete(file); } catch (IOException) { /* temporary-file cleanup is best effort */ }
}

sealed class FirstQueryGate : DbCommandInterceptor
{
    private int _seen;
    private readonly TaskCompletionSource<bool> _entered =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _release =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task Entered => _entered.Task;
    public void Release() => _release.TrySetResult(true);

    public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        if (Interlocked.Exchange(ref _seen, 1) == 0)
        {
            _entered.TrySetResult(true);
            await _release.Task.WaitAsync(cancellationToken);
        }
        return result;
    }
}
