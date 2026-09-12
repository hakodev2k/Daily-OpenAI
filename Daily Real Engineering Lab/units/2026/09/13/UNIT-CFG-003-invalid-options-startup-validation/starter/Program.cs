using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Configuration.AddJsonFile("appsettings.Broken.json", optional: false, reloadOnChange: false);

builder.Services.Configure<ReportOptions>(builder.Configuration.GetSection("Reports"));

var app = builder.Build();
await app.StartAsync();
Console.WriteLine("APP_STARTED");

try
{
    var options = app.Services.GetRequiredService<IOptions<ReportOptions>>().Value;
    var baseUri = new Uri(options.BaseUrl!, UriKind.Absolute);
    var reportUri = new Uri(baseUri, "v1/reports/42");
    Console.WriteLine($"REPORT_URI:{reportUri}");
}
catch (Exception ex)
{
    Console.WriteLine($"REQUEST_FAILED:{ex.GetType().Name}:{ex.Message}");
    await app.StopAsync();
    return 23;
}

await app.StopAsync();
return 0;

public sealed class ReportOptions
{
    public string? BaseUrl { get; set; }
}