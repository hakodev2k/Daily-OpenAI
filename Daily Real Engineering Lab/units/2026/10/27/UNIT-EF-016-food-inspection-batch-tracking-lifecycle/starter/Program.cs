using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

return args.FirstOrDefault()?.ToLowerInvariant() switch
{
    "reproduce" => Reproduce(),
    "verify" => Verify(),
    _ => Usage()
};

static int Usage()
{
    Console.Error.WriteLine("Usage: dotnet run --project starter/InspectionLab.csproj -- reproduce|verify");
    return 2;
}

static int Reproduce()
{
    using var fixture = new Fixture();
    var importer = new InspectionImportService(fixture.Db);
    importer.Import([new Inspection { Id = 2, SiteCode = "BR-02", ReviewStatus = "Imported" }]);

    var tracked = fixture.Db.Inspections.Single(x => x.Id == 1);
    tracked.ReviewStatus = "Approved";
    var writes = fixture.Db.SaveChanges();
    var trackedValue = tracked.ReviewStatus;
    var trackingEnabled = fixture.Db.ChangeTracker.AutoDetectChangesEnabled;
    fixture.Db.ChangeTracker.Clear();
    var persisted = fixture.Db.Inspections.AsNoTracking().Single(x => x.Id == 1).ReviewStatus;
    var imported = fixture.Db.Inspections.AsNoTracking().Any(x => x.Id == 2);

    Console.WriteLine($"ImportPresent={imported}; SaveChangesRows={writes}; Tracked={trackedValue}; Persisted={persisted}; AutoDetectChangesEnabled={trackingEnabled}");
    if (imported && trackedValue == "Approved" && persisted == "Pending" && writes == 0)
    {
        Console.WriteLine("REPRODUCED: review acknowledged in memory but not persisted.");
        return 0;
    }

    Console.Error.WriteLine("Original failure not reproduced (possibly already fixed).");
    return 1;
}

static int Verify()
{
    var failures = new List<string>();
    Check("success path + persisted review", VerifySuccess, failures);
    Check("exception path restores caller setting", VerifyException, failures);
    Check("pre-disabled setting is preserved", VerifyPreservedSetting, failures);
    Console.WriteLine(failures.Count == 0 ? "VERIFY PASS (3/3)" : $"VERIFY FAIL ({failures.Count}/3 failed)");
    return failures.Count == 0 ? 0 : 1;
}

static void Check(string label, Action action, List<string> failures)
{
    try
    {
        action();
        Console.WriteLine($"PASS: {label}");
    }
    catch (Exception ex)
    {
        failures.Add(label);
        Console.Error.WriteLine($"FAIL: {label}: {ex.GetType().Name}: {ex.Message}");
    }
}

static void VerifySuccess()
{
    using var fixture = new Fixture();
    var importer = new InspectionImportService(fixture.Db);
    Require(fixture.Db.ChangeTracker.AutoDetectChangesEnabled, "initial setting must be enabled");
    importer.Import([new Inspection { Id = 2, SiteCode = "BR-02", ReviewStatus = "Imported" }]);
    Require(fixture.Db.ChangeTracker.AutoDetectChangesEnabled, "import must restore caller setting");

    var existing = fixture.Db.Inspections.Single(x => x.Id == 1);
    existing.ReviewStatus = "Approved";
    fixture.Db.SaveChanges();
    fixture.Db.ChangeTracker.Clear();

    Require(fixture.Db.Inspections.AsNoTracking().Single(x => x.Id == 1).ReviewStatus == "Approved",
        "review update was not persisted");
    Require(fixture.Db.Inspections.AsNoTracking().Count() == 2, "imported row missing or duplicated");
}

static void VerifyException()
{
    using var fixture = new Fixture();
    var importer = new InspectionImportService(fixture.Db);
    var threw = false;
    try
    {
        importer.Import([new Inspection { Id = 2, SiteCode = "BR-02", ReviewStatus = "Imported" }], injectFailure: true);
    }
    catch (InvalidOperationException ex) when (ex.Message == "Simulated import failure")
    {
        threw = true;
    }
    Require(threw, "expected injected failure was swallowed");
    Require(fixture.Db.ChangeTracker.AutoDetectChangesEnabled, "setting not restored after exception");
    // This context is discarded after a failed unit of work; pending tracked inserts are not committed.
}

static void VerifyPreservedSetting()
{
    using var fixture = new Fixture();
    fixture.Db.ChangeTracker.AutoDetectChangesEnabled = false;
    var importer = new InspectionImportService(fixture.Db);
    importer.Import([new Inspection { Id = 2, SiteCode = "BR-02", ReviewStatus = "Imported" }]);
    Require(!fixture.Db.ChangeTracker.AutoDetectChangesEnabled, "caller-owned disabled setting was overwritten");
    fixture.Db.ChangeTracker.Clear();
    Require(fixture.Db.Inspections.AsNoTracking().Count() == 2, "batch insert failed");
}

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

internal sealed class Fixture : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    public InspectionDbContext Db { get; }

    public Fixture()
    {
        _connection.Open();
        var options = new DbContextOptionsBuilder<InspectionDbContext>()
            .UseSqlite(_connection)
            .Options;
        Db = new InspectionDbContext(options);
        Db.Database.EnsureCreated();
        Db.Inspections.Add(new Inspection { Id = 1, SiteCode = "BR-01", ReviewStatus = "Pending" });
        Db.SaveChanges();
    }

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }
}

internal sealed class InspectionDbContext(DbContextOptions<InspectionDbContext> options) : DbContext(options)
{
    public DbSet<Inspection> Inspections => Set<Inspection>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inspection>().HasKey(x => x.Id);
        modelBuilder.Entity<Inspection>().Property(x => x.SiteCode).IsRequired();
        modelBuilder.Entity<Inspection>().Property(x => x.ReviewStatus).IsRequired();
    }
}

internal sealed class Inspection
{
    public int Id { get; set; }
    public string SiteCode { get; set; } = "";
    public string ReviewStatus { get; set; } = "";
}
