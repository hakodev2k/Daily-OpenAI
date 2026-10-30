using System.Net;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;

var mode = args.Length == 1 ? args[0] : "";
if (mode != "--reproduce" && mode != "--verify")
{
    Console.Error.WriteLine("Usage: --reproduce | --verify");
    return 2;
}

await using var app = ProbeHostFactory.Create();
await app.StartAsync();
try
{
    var addresses = app.Services.GetRequiredService<IServer>()
        .Features.Get<IServerAddressesFeature>()?.Addresses;
    var address = addresses?.SingleOrDefault(
        x => x.StartsWith("http://127.0.0.1:", StringComparison.OrdinalIgnoreCase));
    if (address is null)
        throw new InvalidOperationException("Không xác định được cổng HTTP local.");

    using var client = new HttpClient { BaseAddress = new Uri(address) };
    var gate = app.Services.GetRequiredService<DependencyGate>();
    var failures = new List<string>();

    async Task ExpectAsync(string phase, HttpStatusCode expectedLive, HttpStatusCode expectedReady)
    {
        using var live = await client.GetAsync("/health/live");
        using var ready = await client.GetAsync("/health/ready");
        Console.WriteLine($"{phase}: live={(int)live.StatusCode}, ready={(int)ready.StatusCode}");
        if (live.StatusCode != expectedLive)
            failures.Add($"{phase}: live expected {(int)expectedLive}, got {(int)live.StatusCode}");
        if (ready.StatusCode != expectedReady)
            failures.Add($"{phase}: ready expected {(int)expectedReady}, got {(int)ready.StatusCode}");
    }

    gate.IsAvailable = true;
    await ExpectAsync("normal", HttpStatusCode.OK, HttpStatusCode.OK);
    gate.IsAvailable = false;
    await ExpectAsync("partner-down",
        mode == "--reproduce" ? HttpStatusCode.ServiceUnavailable : HttpStatusCode.OK,
        HttpStatusCode.ServiceUnavailable);
    gate.IsAvailable = true;
    await ExpectAsync("recovered", HttpStatusCode.OK, HttpStatusCode.OK);

    foreach (var failure in failures)
        Console.Error.WriteLine("FAIL: " + failure);
    Console.WriteLine(failures.Count == 0 ? "PASS: " + mode : "FAIL: " + mode);
    return failures.Count == 0 ? 0 : 1;
}
finally
{
    await app.StopAsync();
}
