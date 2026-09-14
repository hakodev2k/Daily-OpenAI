var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<WarmupState>();
builder.Services.AddHostedService<CatalogWarmupService>();

var app = builder.Build();

app.MapGet("/health/live", () => Results.Ok(new { status = "live" }));
app.MapGet("/health/ready", () => Results.Ok(new { status = "ready" }));
app.MapGet("/catalog/count", (WarmupState state) =>
    state.IsReady ? Results.Ok(new { count = 42 }) : Results.StatusCode(503));

app.Run();

sealed class WarmupState
{
    public volatile bool IsReady;
}

sealed class CatalogWarmupService(WarmupState state) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(4), stoppingToken);
        state.IsReady = true;
    }
}
