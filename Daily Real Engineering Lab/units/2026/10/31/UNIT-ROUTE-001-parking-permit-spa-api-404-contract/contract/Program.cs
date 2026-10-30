using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StarterRoutes = ParkingPermit.Starter.PermitRoutes;
using ReferenceRoutes = ParkingPermit.Reference.PermitRoutes;

var mode = args.Length == 2 && args[0] == "--mode" ? args[1] : "run";
if (mode is not ("run" or "reproduce" or "verify" or "reference"))
{
    Console.Error.WriteLine("Modes: run, reproduce, verify, reference");
    Environment.ExitCode = 2;
    return;
}

var builder = WebApplication.CreateBuilder();
builder.Logging.ClearProviders();
builder.WebHost.UseUrls("http://127.0.0.1:0");

await using var app = builder.Build();
if (mode == "reference")
    ReferenceRoutes.Map(app);
else
    StarterRoutes.Map(app);

await app.StartAsync();
var addresses = app.Services.GetRequiredService<IServer>()
    .Features.Get<IServerAddressesFeature>()?.Addresses;
var baseAddress = addresses?.SingleOrDefault(a =>
    a.StartsWith("http://127.0.0.1:", StringComparison.OrdinalIgnoreCase))
    ?? throw new InvalidOperationException("Kestrel did not bind a loopback URL.");

using var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
async Task<Observation> Probe(string path)
{
    using var response = await client.GetAsync(path);
    return new Observation(
        path,
        response.StatusCode,
        response.Content.Headers.ContentType?.MediaType ?? "",
        await response.Content.ReadAsStringAsync());
}

var knownApi = await Probe("/api/permits/42");
var spa = await Probe("/renewals");
var unknownApi = await Probe("/api/permits/not-a-number");
var unknownApi2 = await Probe("/api/unknown");
var unknownApi3 = await Probe("/api/permits/42/details");
var missingAsset = await Probe("/assets/site.css");

static bool IsJson(Observation x) =>
    x.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase);
static bool IsHtml(Observation x) =>
    x.MediaType.Equals("text/html", StringComparison.OrdinalIgnoreCase);
static bool ApiNotFound(Observation x) =>
    x.Status == HttpStatusCode.NotFound && IsJson(x) &&
    x.Body.Contains("api_route_not_found", StringComparison.Ordinal);

var checks = new List<Check>
{
    new("known-api", knownApi.Status == HttpStatusCode.OK &&
        IsJson(knownApi) && knownApi.Body.Contains("\"permitId\":42", StringComparison.Ordinal), true),
    new("spa-navigation", spa.Status == HttpStatusCode.OK && IsHtml(spa) &&
        spa.Body.Contains("Parking permit portal", StringComparison.Ordinal), true),
    new("missing-static-asset", missingAsset.Status == HttpStatusCode.NotFound, true),
    new("invalid-api-parameter", ApiNotFound(unknownApi), false),
    new("unknown-api-route", ApiNotFound(unknownApi2), false),
    new("unknown-api-subroute", ApiNotFound(unknownApi3), false)
};

foreach (var x in new[] { knownApi, spa, unknownApi, unknownApi2, unknownApi3, missingAsset })
    Console.WriteLine($"{x.Path}: {(int)x.Status} {x.MediaType} body={x.Body[..Math.Min(90, x.Body.Length)]}");

foreach (var c in checks)
    Console.WriteLine($"{(c.Passed ? "PASS" : "FAIL")} {c.Name}");

if (mode == "reproduce")
{
    var observed = checks.Where(x => x.BaselineShouldPass).All(x => x.Passed) &&
                   checks.Where(x => !x.BaselineShouldPass).All(x => !x.Passed);
    Console.WriteLine(observed ? "REPRODUCED: 3 API contract failures; valid routes intact." :
        "NOT REPRODUCED: baseline behavior differs from the scenario.");
    Environment.ExitCode = observed ? 0 : 1;
}
else if (mode is "verify" or "reference")
{
    var passed = checks.All(x => x.Passed);
    Console.WriteLine(passed ? "VERIFIED: all API and SPA contracts pass." :
        "NOT VERIFIED: fix is incomplete or behavior regressed.");
    Environment.ExitCode = passed ? 0 : 1;
}

await app.StopAsync();

internal sealed record Observation(string Path, HttpStatusCode Status, string MediaType, string Body);
internal sealed record Check(string Name, bool Passed, bool BaselineShouldPass);
