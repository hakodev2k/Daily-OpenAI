using System.Net;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel().UseUrls("http://127.0.0.1:0");
await using var app = builder.Build();

var secureRequests = 0;
var authenticatedRequests = 0;
Func<HttpContext, IResult> secureEndpoint = context =>
{
    Interlocked.Increment(ref secureRequests);
    if (context.Request.Headers.Authorization.ToString() == "Bearer demo-token")
    {
        Interlocked.Increment(ref authenticatedRequests);
        return Results.Ok(new { valid = true });
    }
    return Results.Unauthorized();
};

app.MapGet("/partner/direct", secureEndpoint);
app.MapGet("/partner/secure", secureEndpoint);
app.MapGet("/partner/entry", () => Results.Redirect("/partner/secure", preserveMethod: true));
app.MapGet("/partner/external", () => Results.Redirect("http://127.0.0.1:1/sink", preserveMethod: true));

await app.StartAsync();
try
{
    var addresses = app.Services.GetRequiredService<IServer>()
        .Features.Get<IServerAddressesFeature>()?.Addresses
        ?? throw new InvalidOperationException("No Kestrel address feature.");
    var address = addresses.Single(a => a.StartsWith("http://127.0.0.1:", StringComparison.Ordinal));
    using var client = new PartnerGatewayClient(new Uri(address + "/"));
    var mode = args.Length == 2 && args[0] == "--mode" ? args[1] : "smoke";
    int exitCode;
    switch (mode)
    {
        case "smoke":
            exitCode = await SmokeAsync();
            break;
        case "reproduce":
            exitCode = await ReproduceAsync();
            break;
        case "verify":
            exitCode = await VerifyAsync();
            break;
        default:
            exitCode = Fail("Use --mode smoke|reproduce|verify.");
            break;
    }
    return exitCode;

    async Task<int> SmokeAsync()
    {
        var status = await client.CheckAsync("/partner/direct");
        Console.WriteLine($"smoke_status={(int)status}");
        return status == HttpStatusCode.OK ? 0 : Fail("Direct partner call failed.");
    }

    async Task<int> ReproduceAsync()
    {
        try
        {
            var status = await client.CheckAsync("/partner/entry");
            Console.WriteLine($"gateway_status={(int)status}; secure_requests={secureRequests}; authorized={authenticatedRequests}");
            return status == HttpStatusCode.Unauthorized &&
                   secureRequests == 1 && authenticatedRequests == 0
                ? Success("REPRODUCED: gateway path differs from direct authentication.")
                : Fail("Original symptom did not reproduce; starter may already be fixed.");
        }
        catch (Exception ex)
        {
            return Fail($"Unexpected reproduction exception: {ex.GetType().Name}: {ex.Message}");
        }
    }

    async Task<int> VerifyAsync()
    {
        try
        {
            var direct = await client.CheckAsync("/partner/direct");
            var viaGateway = await client.CheckAsync("/partner/entry");
            if (direct != HttpStatusCode.OK || viaGateway != HttpStatusCode.OK ||
                secureRequests != 2 || authenticatedRequests != 2)
                return Fail($"direct={(int)direct}; redirected={(int)viaGateway}; secure_requests={secureRequests}; authorized={authenticatedRequests}. Expected two authenticated 200 responses.");

            var rejected = false;
            try
            {
                await client.CheckAsync("/partner/external");
            }
            catch (InvalidOperationException ex) when (ex.Message.StartsWith("Untrusted redirect", StringComparison.Ordinal))
            {
                rejected = true;
            }

            if (!rejected || secureRequests != 2 || authenticatedRequests != 2)
                return Fail("External-origin redirect was not explicitly rejected by policy.");

            return Success("PASS: direct and redirected paths authorized; external origin blocked.");
        }
        catch (Exception ex)
        {
            return Fail($"Verification exception: {ex.GetType().Name}: {ex.Message}");
        }
    }

    static int Success(string message) { Console.WriteLine(message); return 0; }
    static int Fail(string message) { Console.Error.WriteLine("FAIL: " + message); return 1; }
}
finally
{
    await app.StopAsync();
}
