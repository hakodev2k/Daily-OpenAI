using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<DependencyState>();
builder.Services.AddHealthChecks()
    .AddCheck<RecommendationDependencyCheck>("recommendation-api");

var app = builder.Build();

app.MapGet("/api/profile/{id:int}", (int id) => Results.Ok(new { id, name = $"Customer {id}" }));

app.MapGet("/api/profile/{id:int}/recommendations", (int id, DependencyState state) =>
    state.RecommendationAvailable
        ? Results.Ok(new { customerId = id, items = new[] { "A", "B" } })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable));

app.MapPost("/lab/dependency/{mode}", (string mode, DependencyState state) =>
{
    state.RecommendationAvailable = !string.Equals(mode, "down", StringComparison.OrdinalIgnoreCase);
    return Results.Ok(new { state.RecommendationAvailable });
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.Run();

sealed class DependencyState
{
    public volatile bool RecommendationAvailable = true;
}

sealed class RecommendationDependencyCheck(DependencyState state) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(state.RecommendationAvailable
            ? HealthCheckResult.Healthy("Recommendation API reachable")
            : HealthCheckResult.Unhealthy("Recommendation API unavailable"));
}
