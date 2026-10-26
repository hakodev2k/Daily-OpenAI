using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

var mode = args.Length == 1 ? args[0].ToLowerInvariant() : "";
if (mode is not ("reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project starter/PartnerCookieLab.csproj -- reproduce|verify");
    return 2;
}

var builder = WebApplication.CreateBuilder();
builder.Logging.ClearProviders();
builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Loopback, 0));
var app = builder.Build();

app.MapPost("/auth/{tenant}", (string tenant, HttpContext context) =>
{
    // Synthetic lab cookie; not a production authentication implementation.
    context.Response.Cookies.Append("partner_session", tenant, new CookieOptions { Path = "/" });
    return Results.Ok(new { tenant });
});

app.MapGet("/quote/{tenant}", (string tenant, HttpContext context) =>
{
    var session = context.Request.Cookies["partner_session"] ?? "none";
    // Lab-only diagnostic marker; NEVER expose real session cookies in production logs/headers.
    context.Response.Headers["X-Lab-Observed-Session"] = session;
    if (session == "none") return Results.Unauthorized();
    if (!StringComparer.Ordinal.Equals(session, tenant)) return Results.StatusCode(403);
    return Results.Ok(new { tenant, quote = "synthetic" });
});

await app.StartAsync();
try
{
    var endpoint = app.Urls.Single();
    using var gateway = new TenantGateway(new Uri(endpoint.EndsWith('/') ? endpoint : endpoint + "/"));

    await gateway.AuthenticateAsync("alpha");
    var alphaBefore = await gateway.GetQuoteAsync("alpha");
    var betaBefore = await gateway.GetQuoteAsync("beta");
    Console.WriteLine($"alpha.afterLogin={alphaBefore.StatusCode} session={alphaBefore.ObservedSession}");
    Console.WriteLine($"beta.beforeLogin={betaBefore.StatusCode} session={betaBefore.ObservedSession}");

    if (mode == "reproduce")
    {
        var reproduced = alphaBefore.StatusCode == HttpStatusCode.OK
            && betaBefore.StatusCode == HttpStatusCode.Forbidden
            && betaBefore.ObservedSession == "alpha";
        Console.WriteLine(reproduced ? "REPRODUCED: cross-tenant session observed" : "NOT_REPRODUCED");
        return reproduced ? 0 : 1;
    }

    await gateway.AuthenticateAsync("beta");
    var betaAfter = await gateway.GetQuoteAsync("beta");
    var alphaAfter = await gateway.GetQuoteAsync("alpha");
    Console.WriteLine($"beta.afterLogin={betaAfter.StatusCode} session={betaAfter.ObservedSession}");
    Console.WriteLine($"alpha.afterBetaLogin={alphaAfter.StatusCode} session={alphaAfter.ObservedSession}");

    var valid = alphaBefore.StatusCode == HttpStatusCode.OK
        && alphaBefore.ObservedSession == "alpha"
        && betaBefore.StatusCode == HttpStatusCode.Unauthorized
        && betaBefore.ObservedSession == "none"
        && betaAfter.StatusCode == HttpStatusCode.OK
        && betaAfter.ObservedSession == "beta"
        && alphaAfter.StatusCode == HttpStatusCode.OK
        && alphaAfter.ObservedSession == "alpha";
    Console.WriteLine(valid ? "PASS: tenant sessions isolated" : "FAIL: session boundary regression");
    return valid ? 0 : 1;
}
finally
{
    await app.StopAsync();
}
