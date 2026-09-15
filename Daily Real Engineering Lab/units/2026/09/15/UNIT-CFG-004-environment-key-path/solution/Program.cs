var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var configuredBatchSize = app.Configuration["ExportOptions:BatchSize"] ?? "100";
Console.WriteLine($"Effective export batch size: {configuredBatchSize}");

if (args.Contains("--probe", StringComparer.Ordinal))
{
    return;
}

app.MapGet("/config", () => Results.Ok(new { batchSize = configuredBatchSize }));
app.Run();
