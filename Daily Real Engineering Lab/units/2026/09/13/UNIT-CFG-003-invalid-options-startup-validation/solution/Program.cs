using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Configuration.AddJsonFile("appsettings.Broken.json", optional: false, reloadOnChange: false);

builder.Services
    .AddOptions<ReportOptions>()
    .Bind(builder.Configuration.GetSection("Reports"))
    .Validate(
        options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps),
        "Reports:BaseUrl must be an absolute HTTP(S) URI")
    .ValidateOnStart();

var app = builder.Build();
await app.StartAsync();
Console.WriteLine("APP_STARTED");

var options = app.Services.GetRequiredService<IOptions<ReportOptions>>().Value;
var baseUri = new Uri(options.BaseUrl!, UriKind.Absolute);
var reportUri = new Uri(baseUri, "v1/reports/42");
Console.WriteLine($"REPORT_URI:{reportUri}");

await app.StopAsync();
return 0;

public sealed class ReportOptions
{
    public string? BaseUrl { get; set; }
}