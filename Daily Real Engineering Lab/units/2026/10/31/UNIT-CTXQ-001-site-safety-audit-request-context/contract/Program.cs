using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SiteAuditLab;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var mode = args.FirstOrDefault() ?? "reproduce";
        try
        {
            return mode switch
            {
                "reproduce" => Reproduce(),
                "verify" => Verify(),
                _ => Unknown(mode)
            };
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAIL [{mode}]: {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }

    private static int Unknown(string mode)
    {
        Console.Error.WriteLine($"Unknown mode: {mode}");
        return 2;
    }

    private static int Reproduce()
    {
        var accessor = new HttpContextAccessor();
        var queue = new LocalAuditQueue();
        var dispatch = new AuditDispatchService(accessor, queue);

        Accept(accessor, dispatch, "TENANT-A", "PERMIT-101");
        Accept(accessor, dispatch, "TENANT-B", "PERMIT-202");
        if (queue.PendingCount != 2)
            throw new Exception($"Expected 2 accepted jobs, got {queue.PendingCount}");
        Console.WriteLine("INGRESS: accepted=2; pending=2; both request contexts ended");

        try
        {
            var records = queue.Drain();
            Console.Error.WriteLine($"NOT REPRODUCED: worker returned {records.Count} records");
            return 1;
        }
        catch (InvalidOperationException ex) when (ex.Message == "AUDIT_TENANT_CONTEXT_MISSING")
        {
            Console.WriteLine("REPRODUCED: deferred worker failed AUDIT_TENANT_CONTEXT_MISSING");
            return 0;
        }
    }

    private static int Verify()
    {
        var accessor = new HttpContextAccessor();
        var queue = new LocalAuditQueue();
        var dispatch = new AuditDispatchService(accessor, queue);

        Accept(accessor, dispatch, "TENANT-A", "PERMIT-101");
        Accept(accessor, dispatch, "TENANT-B", "PERMIT-202");
        var records = queue.Drain();
        if (records.Count != 2 ||
            records[0] != new AuditRecord("PERMIT-101", "TENANT-A") ||
            records[1] != new AuditRecord("PERMIT-202", "TENANT-B"))
            throw new Exception("Audit records missing, reordered or attributed to wrong tenant");
        Console.WriteLine("PASS: tenant attribution remains isolated after request completion");

        var before = queue.PendingCount;
        accessor.HttpContext = MakeContext(null);
        try
        {
            try
            {
                dispatch.QueuePermitAudit("PERMIT-INVALID");
                throw new Exception("Missing tenant was accepted into audit queue");
            }
            catch (InvalidOperationException ex) when (ex.Message == "AUDIT_TENANT_CONTEXT_MISSING")
            {
                Console.WriteLine("PASS: invalid request rejected before enqueue");
            }
        }
        finally { accessor.HttpContext = null; }

        if (queue.PendingCount != before)
            throw new Exception("Invalid request left a queued audit job");
        Console.WriteLine("PASS: no invalid work remains queued");
        Console.WriteLine("VERIFIED: all checks ran against learner-editable starter.");
        return 0;
    }

    private static void Accept(
        IHttpContextAccessor accessor, AuditDispatchService dispatch, string tenant, string permit)
    {
        accessor.HttpContext = MakeContext(tenant);
        try
        {
            dispatch.QueuePermitAudit(permit);
            Console.WriteLine($"ACCEPTED: permit={permit}; tenant_claim_present=true");
        }
        finally { accessor.HttpContext = null; }
    }

    private static HttpContext MakeContext(string? tenant)
    {
        var claims = tenant is null
            ? Array.Empty<Claim>()
            : new[] { new Claim("tenant_id", tenant) };
        return new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, "fixture-auth"))
        };
    }
}

internal sealed class LocalAuditQueue : IAuditQueue
{
    private readonly Queue<Func<AuditRecord>> _pending = new();
    public int PendingCount => _pending.Count;
    public void Enqueue(Func<AuditRecord> buildRecord) => _pending.Enqueue(buildRecord);

    public List<AuditRecord> Drain()
    {
        var records = new List<AuditRecord>();
        while (_pending.Count > 0)
            records.Add(_pending.Dequeue().Invoke());
        return records;
    }
}
