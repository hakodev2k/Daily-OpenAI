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
        var tenantId = _context.HttpContext?.User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException("AUDIT_TENANT_CONTEXT_MISSING");

        // Payload bất biến được chụp ở request boundary; worker không đọc HttpContext.
        var record = new AuditRecord(permitId, tenantId);
        _queue.Enqueue(() => record);
    }
}
