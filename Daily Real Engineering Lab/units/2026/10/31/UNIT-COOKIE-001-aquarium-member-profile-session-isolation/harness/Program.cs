using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using PartnerCookieLab;

var mode = args.Length == 1 ? args[0].ToLowerInvariant() : "inspect";
if (mode is not ("inspect" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project harness -- inspect|reproduce|verify");
    return 2;
}

var builder = WebApplication.CreateBuilder();
builder.WebHost.UseUrls("http://127.0.0.1:0");
var app = builder.Build();

app.MapPost("/partner/session/{tenant}", (string tenant, HttpResponse response) =>
{
    if (tenant is not ("coral" or "kelp"))
        return Results.BadRequest();
    response.Cookies.Append("partner_session", tenant, new CookieOptions
    {
        Path = "/partner",
        HttpOnly = true,
        SameSite = SameSiteMode.Strict
    });
    return Results.Ok();
});

app.MapGet("/partner/profile", (HttpRequest request) =>
{
    if (!request.Cookies.TryGetValue("partner_session", out var tenant))
        return Results.Unauthorized();
    return Results.Text(tenant, "text/plain");
});

await app.StartAsync();
try
{
    var addresses = app.Services.GetRequiredService<IServer>()
        .Features.Get<IServerAddressesFeature>()?.Addresses
        ?? throw new InvalidOperationException("No server addresses available");
    var address = addresses.Single(a => a.StartsWith("http://127.0.0.1:", StringComparison.Ordinal));
    using var gateway = new PartnerGateway(new Uri(address.EndsWith('/') ? address : address + "/"));

    var coralLogin = await gateway.StartSessionAsync("coral");
    var coralFirst = await gateway.ReadProfileAsync("coral");
    var kelpBeforeLogin = await gateway.ReadProfileAsync("kelp");
    var kelpLogin = await gateway.StartSessionAsync("kelp");
    var kelpAfterLogin = await gateway.ReadProfileAsync("kelp");
    var coralAfterKelpLogin = await gateway.ReadProfileAsync("coral");

    Console.WriteLine($"Coral login: {coralLogin}");
    Console.WriteLine($"Coral profile: {coralFirst.StatusCode} / {coralFirst.Body}");
    Console.WriteLine($"Kelp before login: {kelpBeforeLogin.StatusCode} / {kelpBeforeLogin.Body}");
    Console.WriteLine($"Kelp login: {kelpLogin}");
    Console.WriteLine($"Kelp profile: {kelpAfterLogin.StatusCode} / {kelpAfterLogin.Body}");
    Console.WriteLine($"Coral after Kelp login: {coralAfterKelpLogin.StatusCode} / {coralAfterKelpLogin.Body}");

    var reproduced =
        coralLogin == 200 && coralFirst is { StatusCode: 200, Body: "coral" } &&
        kelpBeforeLogin is { StatusCode: 200, Body: "coral" } &&
        kelpLogin == 200 && kelpAfterLogin is { StatusCode: 200, Body: "kelp" } &&
        coralAfterKelpLogin is { StatusCode: 200, Body: "kelp" };

    var verified =
        coralLogin == 200 && coralFirst is { StatusCode: 200, Body: "coral" } &&
        kelpBeforeLogin.StatusCode == 401 &&
        kelpLogin == 200 && kelpAfterLogin is { StatusCode: 200, Body: "kelp" } &&
        coralAfterKelpLogin is { StatusCode: 200, Body: "coral" };

    if (mode == "reproduce")
    {
        Console.WriteLine(reproduced ? "REPRODUCED: cross-tenant session leak" : "NOT REPRODUCED: inspect output");
        return reproduced ? 0 : 1;
    }

    if (mode == "verify")
    {
        Console.WriteLine(verified ? "PASS: tenant session isolation" : "FAIL: tenant session isolation");
        return verified ? 0 : 1;
    }

    return 0;
}
finally
{
    await app.StopAsync();
    await app.DisposeAsync();
}
