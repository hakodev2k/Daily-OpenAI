using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [reproduce|verify]");
    return 64;
}

bool verify = args[0] == "verify";
var builder = WebApplication.CreateBuilder(Array.Empty<string>());
builder.Logging.ClearProviders();
builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Loopback, 0));

await using var app = builder.Build();
app.MapGet("/api/waveforms/{id}", WaveformEndpoint.HandleAsync);
await app.StartAsync();

var addresses = app.Services.GetRequiredService<IServer>()
    .Features.Get<IServerAddressesFeature>()?.Addresses;
if (addresses is null || addresses.Count != 1)
    throw new InvalidOperationException("Expected exactly one loopback listener");

using var client = new HttpClient
{
    BaseAddress = new Uri(addresses.Single()),
    Timeout = TimeSpan.FromSeconds(10)
};

var cases = new[]
{
    new TestCase("full", null, 200, "ABCDEFGHIJ", null),
    new TestCase("bounded", "bytes=3-6", 206, "DEFG", "bytes 3-6/10"),
    new TestCase("one-byte", "bytes=0-0", 206, "A", "bytes 0-0/10"),
    new TestCase("open-end", "bytes=7-", 206, "HIJ", "bytes 7-9/10"),
    new TestCase("suffix", "bytes=-4", 206, "GHIJ", "bytes 6-9/10"),
    new TestCase("unsatisfiable", "bytes=50-60", 416, "", "bytes */10")
};

var observations = new Dictionary<string, Observation>();
bool allCorrect = true;
foreach (var test in cases)
{
    using var request = new HttpRequestMessage(HttpMethod.Get, "/api/waveforms/SEIS-42");
    if (test.Range is not null)
        request.Headers.TryAddWithoutValidation("Range", test.Range);

    using var response = await client.SendAsync(request);
    var body = Encoding.ASCII.GetString(await response.Content.ReadAsByteArrayAsync());
    var contentRange = response.Content.Headers.ContentRange?.ToString();
    var observation = new Observation((int)response.StatusCode, body, contentRange);
    observations.Add(test.Name, observation);

    bool correct = observation.Status == test.Status
        && observation.Body == test.Body
        && observation.ContentRange == test.ContentRange;
    allCorrect &= correct;

    Console.WriteLine(test.Name + ": status=" + observation.Status
        + " Content-Range=" + (contentRange ?? "<none>")
        + " body='" + body + "' " + (correct ? "OK" : "MISMATCH"));
}

await app.StopAsync();

if (!verify)
{
    var bounded = observations["bounded"];
    bool reproduced = bounded.Status == 206
        && bounded.ContentRange == "bytes 3-6/10"
        && bounded.Body == "DEF"
        && observations["full"].Body == "ABCDEFGHIJ";
    Console.WriteLine(reproduced ? "REPRODUCED: 206 segment differs from advertised range"
        : "NOT REPRODUCED: starter symptom not observed");
    return reproduced ? 0 : 2;
}

Console.WriteLine(allCorrect
    ? "PASS: full, bounded, single-byte, open-ended, suffix and unsatisfiable ranges"
    : "FAIL: at least one learner-editable endpoint contract remains incorrect");
return allCorrect ? 0 : 1;

internal sealed record TestCase(
    string Name, string? Range, int Status, string Body, string? ContentRange);

internal sealed record Observation(int Status, string Body, string? ContentRange);
