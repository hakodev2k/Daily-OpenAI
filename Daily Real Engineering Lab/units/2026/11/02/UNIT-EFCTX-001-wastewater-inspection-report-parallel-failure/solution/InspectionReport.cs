using Microsoft.EntityFrameworkCore;

namespace WastewaterLab;

// Reference implementation: replace only the service implementation in
// starter/InspectionReport.cs; keep the model and DbContext unchanged.
public sealed class InspectionReportService
{
    private readonly Func<InspectionDbContext> _newContext;

    public InspectionReportService(Func<InspectionDbContext> newContext)
        => _newContext = newContext;

    public async Task<InspectionReport> BuildAsync(CancellationToken cancellationToken = default)
    {
        await using var openDb = _newContext();
        await using var closedDb = _newContext();

        var openTask = openDb.Inspections.AsNoTracking()
            .Where(x => x.Status == "Open")
            .OrderBy(x => x.TicketId)
            .Select(x => x.TicketId)
            .ToListAsync(cancellationToken);

        var closedTask = closedDb.Inspections.AsNoTracking()
            .Where(x => x.Status == "Closed")
            .OrderBy(x => x.TicketId)
            .Select(x => x.TicketId)
            .ToListAsync(cancellationToken);

        await Task.WhenAll(openTask, closedTask);
        return new InspectionReport(openTask.Result, closedTask.Result);
    }
}
