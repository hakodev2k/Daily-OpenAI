using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<FakeCatalogDependency>();
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live", "ready" })
    .AddCheck<CatalogDependencyHealthCheck>("catalog", tags: new[] { "ready" });

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "inventory-api" }));
app.MapPost("/dependency/{state}", (string state, FakeCatalogDependency dependency) =>
{
    dependency.IsAvailable = string.Equals(state, "up", StringComparison.OrdinalIgnoreCase);
    return Results.Ok(new { dependency.IsAvailable });
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});

app.Run("http://127.0.0.1:5088");

sealed class FakeCatalogDependency
{
    public volatile bool IsAvailable = true;
}

sealed class CatalogDependencyHealthCheck(FakeCatalogDependency dependency) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            dependency.IsAvailable
                ? HealthCheckResult.Healthy("Catalog reachable")
                : HealthCheckResult.Unhealthy("Catalog unavailable"));
    }
}
