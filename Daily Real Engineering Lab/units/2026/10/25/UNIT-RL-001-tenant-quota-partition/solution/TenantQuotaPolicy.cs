using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;

namespace TenantQuotaLab;

/// <summary>
/// Reference implementation. Copy the relevant policy changes to starter
/// only after completing your own investigation.
/// </summary>
public static class TenantQuotaPolicy
{
    public static PartitionedRateLimiter<HttpContext> Create()
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            string? tenantId = context.User.FindFirst("tenant_id")?.Value;
            string key = string.IsNullOrWhiteSpace(tenantId)
                ? "anonymous"
                : $"tenant:{tenantId}";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: key,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 2,
                    Window = TimeSpan.FromMinutes(5),
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
        });
    }
}
