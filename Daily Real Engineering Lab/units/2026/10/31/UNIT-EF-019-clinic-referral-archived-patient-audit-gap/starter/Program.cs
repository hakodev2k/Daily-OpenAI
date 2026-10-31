using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var mode = args.FirstOrDefault() ?? "--run";
if (mode is not ("--run" or "--reproduce" or "--verify"))
{
    Console.Error.WriteLine("Usage: --run | --reproduce | --verify");
    return 2;
}

await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();
var options = new DbContextOptionsBuilder<ReferralDb>()
    .UseSqlite(connection)
    .Options;

await using var db = new ReferralDb(options);
await db.Database.EnsureCreatedAsync();

db.Patients.AddRange(
    new Patient { Id = 1, TenantId = "TENANT-ALPHA", DisplayName = "Patient A", IsArchived = false },
    new Patient { Id = 2, TenantId = "TENANT-ALPHA", DisplayName = "Private archived profile", IsArchived = true },
    new Patient { Id = 3, TenantId = "TENANT-BETA", DisplayName = "Patient C", IsArchived = false });
db.ReferralEvents.AddRange(
    new ReferralEvent { Id = 101, EventKey = "EVT-101", PatientId = 1, TenantId = "TENANT-ALPHA", Action = "CREATED" },
    new ReferralEvent { Id = 102, EventKey = "EVT-102", PatientId = 2, TenantId = "TENANT-ALPHA", Action = "TRANSFERRED" },
    new ReferralEvent { Id = 999, EventKey = "EVT-999", PatientId = 3, TenantId = "TENANT-BETA", Action = "CREATED" });
await db.SaveChangesAsync();
db.ChangeTracker.Clear();

const string tenant = "TENANT-ALPHA";
var expectedStored = await db.ReferralEvents.CountAsync(x => x.TenantId == tenant);
var exporter = new ReferralAuditExporter(db);
Console.WriteLine($"Stored events for {tenant}: {expectedStored}");
Console.WriteLine("Generated SQL:");
Console.WriteLine(exporter.DiagnosticSql(tenant));
var rows = await exporter.ExportAsync(tenant);
Console.WriteLine($"Exported rows: {rows.Length}");
Console.WriteLine("IDs: " + string.Join(",", rows.Select(x => x.EventKey)));

var ids = rows.Select(x => x.EventKey).OrderBy(x => x, StringComparer.Ordinal).ToArray();
var originalSymptom = expectedStored == 2 && ids.SequenceEqual(new[] { "EVT-101" });
var correct = expectedStored == 2
    && ids.SequenceEqual(new[] { "EVT-101", "EVT-102" })
    && db.ChangeTracker.Entries<Patient>().Count() == 0;

if (mode == "--reproduce")
{
    Console.WriteLine(originalSymptom ? "REPRODUCED: audit export is missing a historical event." : "NOT REPRODUCED");
    return originalSymptom ? 0 : 1;
}
if (mode == "--verify")
{
    Console.WriteLine(correct ? "PASS: complete tenant-scoped audit export without patient materialization." : "FAIL: audit export contract violated.");
    return correct ? 0 : 1;
}
return 0;

public sealed class ReferralAuditExporter(ReferralDb db)
{
    public string DiagnosticSql(string tenantId) =>
        db.ReferralEvents
          .Where(x => x.TenantId == tenantId)
          .Include(x => x.Patient)
          .OrderBy(x => x.Id)
          .ToQueryString();

    public async Task<AuditRow[]> ExportAsync(string tenantId)
    {
        var events = await db.ReferralEvents
            .Where(x => x.TenantId == tenantId)
            .Include(x => x.Patient)
            .OrderBy(x => x.Id)
            .ToListAsync();

        return events.Select(x => new AuditRow(x.EventKey, x.PatientId, x.Action)).ToArray();
    }
}

public sealed record AuditRow(string EventKey, int PatientId, string Action);

public sealed class Patient
{
    public int Id { get; set; }
    public string TenantId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public bool IsArchived { get; set; }
}

public sealed class ReferralEvent
{
    public int Id { get; set; }
    public string EventKey { get; set; } = "";
    public string TenantId { get; set; } = "";
    public string Action { get; set; } = "";
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
}

public sealed class ReferralDb(DbContextOptions<ReferralDb> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<ReferralEvent> ReferralEvents => Set<ReferralEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasQueryFilter(p => !p.IsArchived);
        modelBuilder.Entity<ReferralEvent>()
            .HasOne(x => x.Patient)
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .IsRequired();
    }
}
