using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

var builder = WebApplication.CreateBuilder();
builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Logging.ClearProviders();
var app = builder.Build();

app.Use(async (context, next) =>
{
    await next();
    context.Response.Headers["X-Request-Trace"] = context.TraceIdentifier;
});

app.MapGet("/api/arrivals", async context =>
{
    const string payload = "{\"stop\":\"B12\",\"minutes\":4}";
    context.Response.ContentType = "application/json";
    context.Response.ContentLength = Encoding.UTF8.GetByteCount(payload);
    await context.Response.WriteAsync(payload);
});
app.MapGet("/api/service-alert", async context =>
{
    const string payload = "{\"line\":\"M2\",\"disrupted\":false}";
    context.Response.ContentType = "application/json";
    context.Response.ContentLength = Encoding.UTF8.GetByteCount(payload);
    await context.Response.WriteAsync(payload);
});

await app.StartAsync();
try
{
    var server = app.Services.GetRequiredService<IServer>();
    var addresses = server.Features.Get<IServerAddressesFeature>()
        ?? throw new InvalidOperationException("Kestrel addresses unavailable");
    var address = addresses.Addresses.Single(
        a => a.StartsWith("http://127.0.0.1:", StringComparison.Ordinal));
    using var client = new HttpClient
    {
        BaseAddress = new Uri(address),
        Timeout = TimeSpan.FromSeconds(5)
    };
    var arrivals = await ProbeAsync(client, "/api/arrivals",
        "{\"stop\":\"B12\",\"minutes\":4}");
    var alerts = await ProbeAsync(client, "/api/service-alert",
        "{\"line\":\"M2\",\"disrupted\":false}");
    var passed = arrivals.Valid && alerts.Valid
        && !string.Equals(arrivals.Trace, alerts.Trace, StringComparison.Ordinal);
    Console.WriteLine(passed ? "CONTRACT_OK" : "OBSERVED_FAILURE");
    return (args.FirstOrDefault() ?? "inspect") switch
    {
        "reproduce" => passed ? 2 : 0,
        "verify" => passed ? 0 : 3,
        _ => 0
    };
}
finally
{
    await app.StopAsync();
}

static async Task<(bool Valid, string? Trace)> ProbeAsync(
    HttpClient client, string path, string expectedBody)
{
    try
    {
        using var response = await client.GetAsync(path);
        var body = await response.Content.ReadAsStringAsync();
        var values = response.Headers.TryGetValues("X-Request-Trace", out var headers)
            ? headers!.ToArray() : Array.Empty<string>();
        var trace = values.Length == 1 ? values[0] : null;
        var bodyMatches = string.Equals(body, expectedBody, StringComparison.Ordinal);
        var valid = response.StatusCode == HttpStatusCode.OK
            && bodyMatches && !string.IsNullOrWhiteSpace(trace);
        Console.WriteLine($"{path}: status={(int)response.StatusCode}, " +
            $"bodyMatches={bodyMatches}, traceHeaderPresent={!string.IsNullOrWhiteSpace(trace)}");
        return (valid, trace);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{path}: transport failure ({ex.GetType().Name})");
        return (false, null);
    }
}
