using Microsoft.EntityFrameworkCore;

namespace StreetlightLab;

public sealed class WorkOrderReader
{
    private readonly IDbContextFactory<WorkOrdersContext> _factory;

    public WorkOrderReader(IDbContextFactory<WorkOrdersContext> factory)
    {
        _factory = factory;
    }

    public IReadOnlyList<string> GetOpenCodes(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("Tenant identity is required.", nameof(tenantId));

        using var db = _factory.CreateDbContext();

        // Tenant identity là state của mỗi lần borrow, không phải state của pool.
        db.TenantId = tenantId;

        return db.WorkOrders.AsNoTracking()
            .Where(x => x.IsOpen)
            .OrderBy(x => x.Code)
            .Select(x => x.Code)
            .ToArray();
    }
}
