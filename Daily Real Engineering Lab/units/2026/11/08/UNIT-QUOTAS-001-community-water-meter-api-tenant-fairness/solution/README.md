# Reference — chỉ mở sau khi verify

## Root cause
Bộ đếm global nhận 8 request đầu theo thứ tự đến; 6 request của bulk tiêu phần lớn budget trước khi clinic/school đến. Một giới hạn global đảm bảo capacity nhưng không đảm bảo tenant isolation.

## Minimal reference implementation
Thay phương thức `Admit` trong `starter/Program.cs` bằng:

```csharp
static IEnumerable<Decision> Admit(IEnumerable<Request> requests)
{
    var limits = new Dictionary<string, int> {
        ["bulk"] = 4, ["clinic"] = 2, ["school"] = 2
    };
    var used = new Dictionary<string, int>();
    int total = 0;
    foreach (var r in requests)
    {
        int current = used.GetValueOrDefault(r.Tenant);
        bool allowed = total < 8 && limits.TryGetValue(r.Tenant, out int limit) && current < limit;
        if (allowed) { total++; used[r.Tenant] = current + 1; }
        yield return new(r, allowed, allowed ? "ok" : "quota");
    }
}
```

Expected: bulk 4/8, clinic 2/2, school 2/2, total 8. This is a **fixture-specific reference**, not a complete production limiter.

## Competing designs
1. Static tenant reservation: predictable isolation; unused tenant capacity may be wasted; requires configuration and onboarding.
2. Weighted fair queue + global token bucket: better work conservation and burst handling; introduces queue latency, scheduler complexity and cross-node coordination.
3. Global-only quota: simplest capacity control but violates the stated isolation SLO.

## Wrong fixes
- Raise global quota to 12: violates upstream capacity.
- Retry 429 immediately: can amplify load and starve small tenants further.
- Rely on client-supplied tenant name without authentication: enables quota evasion.
- Per-node in-memory limits on multiple replicas: effective global capacity can multiply.

## Production checklist
Authenticated tenant identity, per-tenant key cardinality, window/burst semantics, global budget, bounded queue, HTTP 429 and Retry-After, telemetry for accepted/rejected by tenant, distributed consistency, and a documented policy for idle reserved capacity.

## Validation note
Static consistency review only. No claim of running dotnet or PowerShell in this publication.
