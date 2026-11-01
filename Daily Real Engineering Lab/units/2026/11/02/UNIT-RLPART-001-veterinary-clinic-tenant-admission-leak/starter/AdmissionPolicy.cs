using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;

namespace TenantGateLab;

public static class AdmissionPolicy
{
    public static PartitionedRateLimiter<HttpContext> Create() =>
        PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                context.Request.Path.Value ?? "/",
                static _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 2,
                    Window = TimeSpan.FromMinutes(10),
                    QueueLimit = 0,
                    AutoReplenishment = false
                }));
}
