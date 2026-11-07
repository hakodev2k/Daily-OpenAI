using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;

namespace TenantQuotaLab;

/// <summary>
/// Shared quota policy for the document-rendering API.
/// Registered in production with RateLimiterOptions.GlobalLimiter.
/// The identity used for quota partitioning is intentionally under investigation.
/// </summary>
public static class TenantQuotaPolicy
{
    public static PartitionedRateLimiter<HttpContext> Create()
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Request.Path.Value ?? "/",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 2,
                    Window = TimeSpan.FromMinutes(5),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));
    }
}
