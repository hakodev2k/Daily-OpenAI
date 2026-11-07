using Microsoft.AspNetCore.Http;

namespace SiteAuditLab;

public sealed record AuditRecord(string PermitId, string TenantId);

public interface IAuditQueue
{
    void Enqueue(Func<AuditRecord> buildRecord);
}

public sealed class AuditDispatchService
{
    private readonly IHttpContextAccessor _context;
    private readonly IAuditQueue _queue;

    public AuditDispatchService(IHttpContextAccessor context, IAuditQueue queue)
    {
        _context = context;
        _queue = queue;
    }

    public void QueuePermitAudit(string permitId)
    {
        // Worker thực hiện công việc sau khi API đã trả accepted.
        _queue.Enqueue(() =>
        {
            var tenantId = _context.HttpContext?.User.FindFirst("tenant_id")?.Value;
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new InvalidOperationException("AUDIT_TENANT_CONTEXT_MISSING");
            return new AuditRecord(permitId, tenantId);
        });
    }
}
