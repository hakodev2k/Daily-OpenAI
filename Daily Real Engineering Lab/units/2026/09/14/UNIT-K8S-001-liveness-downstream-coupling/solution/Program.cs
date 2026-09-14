var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var dependency = new DependencyState();

app.MapGet("/health/live", () => Results.Ok(new { status = "Healthy" }));

app.MapGet("/health/ready", () =>
{
    return dependency.IsHealthy
        ? Results.Ok(new { status = "Healthy" })
        : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
});

app.MapPost("/admin/dependency/{healthy:bool}", (bool healthy) =>
{
    dependency.IsHealthy = healthy;
    return Results.Ok(new { dependencyHealthy = dependency.IsHealthy });
});

app.MapGet("/api/catalog", () =>
{
    if (!dependency.IsHealthy)
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }

    return Results.Ok(new[] { "keyboard", "monitor", "dock" });
});

app.Run();

sealed class DependencyState
{
    public volatile bool IsHealthy = true;
}
