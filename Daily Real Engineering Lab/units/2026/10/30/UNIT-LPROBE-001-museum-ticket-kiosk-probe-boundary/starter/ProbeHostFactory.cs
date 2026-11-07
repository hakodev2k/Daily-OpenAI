using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public static class ProbeHostFactory
{
    public static WebApplication Create()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddSingleton<DependencyGate>();
        builder.Services.AddHealthChecks()
            .AddCheck<ProcessHealthCheck>("process", tags: new[] { "live" })
            .AddCheck<PartnerScoringCheck>("partner-scoring", tags: new[] { "ready" });

        var app = builder.Build();
        app.MapGet("/", () => "Museum ticket validation service");
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => true
        });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration =>
                registration.Tags.Contains("live") || registration.Tags.Contains("ready")
        });
        return app;
    }
}

public sealed class DependencyGate
{
    public volatile bool IsAvailable = true;
}

public sealed class ProcessHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(HealthCheckResult.Healthy("Process is responsive"));
}

public sealed class PartnerScoringCheck(DependencyGate gate) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(gate.IsAvailable
            ? HealthCheckResult.Healthy("Partner scoring reachable")
            : HealthCheckResult.Unhealthy("Partner scoring unavailable"));
}
