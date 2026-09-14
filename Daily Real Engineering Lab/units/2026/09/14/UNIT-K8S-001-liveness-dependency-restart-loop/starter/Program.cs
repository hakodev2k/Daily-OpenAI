using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<FakeCatalogDependency>();
builder.Services.AddHealthChecks()
    .AddCheck<CatalogDependencyHealthCheck>("catalog");

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "inventory-api" }));

app.MapPost("/dependency/{state}", (string state, FakeCatalogDependency dependency) =>
{
    dependency.IsAvailable = string.Equals(state, "up", StringComparison.OrdinalIgnoreCase);
    return Results.Ok(new { dependency.IsAvailable });
});

// Investigation note:
// Consider what an orchestrator should learn from each health endpoint.
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

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
