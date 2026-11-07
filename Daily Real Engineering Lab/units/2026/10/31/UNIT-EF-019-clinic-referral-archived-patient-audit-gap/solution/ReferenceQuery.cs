using Microsoft.EntityFrameworkCore;

// Reference replacement: thay toàn bộ class ReferralAuditExporter trong starter/Program.cs.
// Các type AuditRow và ReferralDb vẫn dùng từ starter.
public sealed class ReferralAuditExporter(ReferralDb db)
{
    private IQueryable<AuditRow> BuildQuery(string tenantId) =>
        db.ReferralEvents
          .AsNoTracking()
          .Where(x => x.TenantId == tenantId)
          .OrderBy(x => x.Id)
          .Select(x => new AuditRow(x.EventKey, x.PatientId, x.Action));

    public string DiagnosticSql(string tenantId) => BuildQuery(tenantId).ToQueryString();

    public Task<AuditRow[]> ExportAsync(string tenantId) =>
        BuildQuery(tenantId).ToArrayAsync();
}
