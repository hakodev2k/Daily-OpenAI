using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

const int PortA = 5211;
const int PortB = 5212;

var mode = args.FirstOrDefault()?.ToLowerInvariant() ?? "run";
var appA = await StartBackendAsync(PortA, "A");
var appB = await StartBackendAsync(PortB, "B");

try
{
    EndpointRegistry.CurrentPort = PortA;
    using var client = ClientFactory.Create();

    var firstBackend = await CallAsync(client);
    Console.WriteLine($"request-1 backend={firstBackend} registry=A");

    EndpointRegistry.CurrentPort = PortB;
    Console.WriteLine("registry switched to B");

    await Task.Delay(mode == "verify" ? 350 : 50);

    var secondBackend = await CallAsync(client);
    Console.WriteLine($"request-2 backend={secondBackend} registry=B");

    return mode switch
    {
        "reproduce" when firstBackend == "A" && secondBackend == "A" => 0,
        "verify" when firstBackend == "A" && secondBackend == "B" => 0,
        "run" => 0,
        _ => 1
    };
}
finally
{
    await appA.StopAsync();
    await appB.StopAsync();
    await appA.DisposeAsync();
    await appB.DisposeAsync();
}

static async Task<string> CallAsync(HttpClient client)
{
    using var response = await client.GetAsync("http://partner.internal/");
    response.EnsureSuccessStatusCode();
    return response.Headers.GetValues("X-Backend").Single();
}

static async Task<WebApplication> StartBackendAsync(int port, string name)
{
    var builder = WebApplication.CreateSlimBuilder();
    builder.WebHost.UseUrls($"http://127.0.0.1:{port}");

    var app = builder.Build();
    app.MapGet("/", (HttpContext context) =>
    {
        context.Response.Headers["X-Backend"] = name;
        return Results.Text($"backend-{name}");
    });

    await app.StartAsync();
    return app;
}
