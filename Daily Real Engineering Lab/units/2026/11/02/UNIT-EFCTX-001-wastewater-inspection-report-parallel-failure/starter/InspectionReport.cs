using Microsoft.EntityFrameworkCore;

namespace WastewaterLab;

public sealed class Inspection
{
    public int Id { get; set; }
    public string TicketId { get; set; } = "";
    public string Status { get; set; } = "";
}

public sealed class InspectionDbContext : DbContext
{
    public InspectionDbContext(DbContextOptions<InspectionDbContext> options) : base(options) { }

    public DbSet<Inspection> Inspections => Set<Inspection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inspection>().HasKey(x => x.Id);
        modelBuilder.Entity<Inspection>().Property(x => x.TicketId).IsRequired();
        modelBuilder.Entity<Inspection>().Property(x => x.Status).IsRequired();
    }
}

public sealed record InspectionReport(
    IReadOnlyList<string> OpenTickets,
    IReadOnlyList<string> ClosedTickets);

public sealed class InspectionReportService
{
    private readonly Func<InspectionDbContext> _newContext;

    public InspectionReportService(Func<InspectionDbContext> newContext)
        => _newContext = newContext;

    public async Task<InspectionReport> BuildAsync(CancellationToken cancellationToken = default)
    {
        await using var db = _newContext();

        // Both independent dashboard sections are requested at once.
        var openTask = db.Inspections.AsNoTracking()
            .Where(x => x.Status == "Open")
            .OrderBy(x => x.TicketId)
            .Select(x => x.TicketId)
            .ToListAsync(cancellationToken);

        var closedTask = db.Inspections.AsNoTracking()
            .Where(x => x.Status == "Closed")
            .OrderBy(x => x.TicketId)
            .Select(x => x.TicketId)
            .ToListAsync(cancellationToken);

        await Task.WhenAll(openTask, closedTask);
        return new InspectionReport(openTask.Result, closedTask.Result);
    }
}
