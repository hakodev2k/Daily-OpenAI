using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;

namespace TenantGateLab;

public static class AdmissionPolicy
{
    public static PartitionedRateLimiter<HttpContext> Create() =>
        PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            string? clinic = context.User.FindFirst("clinic_id")?.Value;
            if (context.User.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(clinic))
                throw new InvalidOperationException(
                    "Authenticated clinic claim required before rate limiting.");

            return RateLimitPartition.GetFixedWindowLimiter(
                clinic,
                static _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 2,
                    Window = TimeSpan.FromMinutes(10),
                    QueueLimit = 0,
                    AutoReplenishment = false
                });
        });
}
