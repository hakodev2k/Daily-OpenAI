using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var mode = args.Length == 0 ? "run" : args[0].ToLowerInvariant();

if (mode == "reproduce")
{
    var actual = await InventoryExercise.RunAsync(10, 2, 1);
    Console.WriteLine("Reproduce: expected available=7, observed available=" + actual.Available
        + ", saleRejected=" + actual.SaleRejected);
    if (actual.Available != 9 || actual.SaleRejected)
    {
        Console.Error.WriteLine("Original symptom not reproduced. This command expects the unmodified starter.");
        Environment.ExitCode = 1;
    }
}
else if (mode == "verify")
{
    var fixtures = new (int Opening, int Reserved, int Sold, Outcome Expected)[]
    {
        (10, 2, 1, new Outcome(7, false)),
        (12, 3, 4, new Outcome(5, false)),
        (3, 2, 2, new Outcome(1, true))
    };
    var failures = 0;
    foreach (var fixture in fixtures)
    {
        var actual = await InventoryExercise.RunAsync(fixture.Opening, fixture.Reserved, fixture.Sold);
        var passed = actual == fixture.Expected;
        Console.WriteLine((passed ? "PASS" : "FAIL") + " opening=" + fixture.Opening
            + " reserved=" + fixture.Reserved + " sold=" + fixture.Sold
            + " expected=" + fixture.Expected + " observed=" + actual);
        if (!passed) failures++;
    }
    if (failures != 0)
    {
        Console.Error.WriteLine(failures + " inventory-contract fixture(s) failed.");
        Environment.ExitCode = 1;
    }
}
else if (mode == "run")
{
    var actual = await InventoryExercise.RunAsync(10, 2, 1);
    Console.WriteLine("Rental counter: opening=10, stageReserved=2, counterSold=1");
    Console.WriteLine("Persisted available=" + actual.Available + "; saleRejected=" + actual.SaleRejected);
}
else
{
    Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
    Environment.ExitCode = 2;
}

internal sealed record Outcome(int Available, bool SaleRejected);

internal sealed class PropStock
{
    public int Id { get; set; }
    public int Available { get; set; }
}

internal sealed class PropsDb : DbContext
{
    public PropsDb(DbContextOptions<PropsDb> options) : base(options) { }
    public DbSet<PropStock> Props => Set<PropStock>();
    protected override void OnModelCreating(ModelBuilder builder)
        => builder.Entity<PropStock>().HasKey(x => x.Id);
}

internal static class InventoryExercise
{
    public static async Task<Outcome> RunAsync(int opening, int reserved, int sold)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<PropsDb>().UseSqlite(connection).Options;
        await using var db = new PropsDb(options);
        await db.Database.EnsureCreatedAsync();
        db.Props.Add(new PropStock { Id = 1, Available = opening });
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        // Counter clerk opens the current item before the stage team allocates equipment.
        var tracked = await db.Props.SingleAsync(x => x.Id == 1);

        // The stage team's batch reservation is recorded immediately.
        var changed = await db.Props.Where(x => x.Id == 1 && x.Available >= reserved)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(p => p.Available, p => p.Available - reserved));
        if (changed != 1) throw new InvalidOperationException("Stage allocation was rejected.");

        // The clerk completes a rental against the same request scope.
        await db.Entry(tracked).ReloadAsync();
        var rejected = tracked.Available < sold;
        if (!rejected)
        {
            tracked.Available -= sold;
            await db.SaveChangesAsync();
        }

        var persisted = await db.Props.AsNoTracking().SingleAsync(x => x.Id == 1);
        return new Outcome(persisted.Available, rejected);
    }
}
