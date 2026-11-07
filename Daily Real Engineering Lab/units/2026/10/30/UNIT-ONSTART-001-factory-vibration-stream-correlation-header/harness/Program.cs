using System.Net;
using EngineeringLab;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

string mode = args.Length == 1 ? args[0].ToLowerInvariant() : "run";
if (mode is not ("run" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project harness -- [run|reproduce|verify]");
    return 2;
}

await using var app = FactoryDashboard.Build();
await app.StartAsync();

try
{
    var server = app.Services.GetRequiredService<IServer>();
    var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses
        ?? throw new InvalidOperationException("No listening addresses were reported.");
    var address = addresses.Single(a => a.StartsWith("http://127.0.0.1:", StringComparison.Ordinal));

    using var client = new HttpClient
    {
        BaseAddress = new Uri(address),
        Timeout = TimeSpan.FromSeconds(10)
    };

    using var health = await client.GetAsync("/health", HttpCompletionOption.ResponseHeadersRead);
    using var live = await client.GetAsync("/api/readings/live", HttpCompletionOption.ResponseHeadersRead);
    string body = await live.Content.ReadAsStringAsync();

    static bool HasTrace(HttpResponseMessage response) =>
        response.Headers.TryGetValues("X-Trace-Id", out var values)
        && values.Any(v => !string.IsNullOrWhiteSpace(v));

    bool healthTrace = HasTrace(health);
    bool liveTrace = HasTrace(live);
    bool contract = health.StatusCode == HttpStatusCode.NoContent
        && live.StatusCode == HttpStatusCode.OK
        && live.Content.Headers.ContentType?.MediaType == "text/event-stream"
        && body == "event: vibration\ndata: 2.4\n\n";

    Console.WriteLine($"health={((int)health.StatusCode)} trace={healthTrace}");
    Console.WriteLine($"live={((int)live.StatusCode)} trace={liveTrace} bodyContract={contract}");

    bool passed = mode switch
    {
        "reproduce" => contract && healthTrace && !liveTrace,
        "verify" => contract && healthTrace && liveTrace,
        _ => contract
    };
    Console.WriteLine(passed ? "PASS" : "FAIL");
    return passed ? 0 : 1;
}
finally
{
    await app.StopAsync();
}
