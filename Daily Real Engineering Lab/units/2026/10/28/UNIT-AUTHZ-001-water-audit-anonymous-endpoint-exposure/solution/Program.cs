using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;

var mode = args.FirstOrDefault() ?? "--serve";
if (mode is not ("--serve" or "--reproduce" or "--verify"))
{
    Console.Error.WriteLine("Use --serve, --reproduce or --verify.");
    return 2;
}

var builder = WebApplication.CreateBuilder(Array.Empty<string>());
builder.WebHost.ConfigureKestrel(server => server.Listen(IPAddress.Loopback, 0));
builder.Logging.ClearProviders();

builder.Services.AddAuthentication("Demo")
    .AddScheme<AuthenticationSchemeOptions, DemoAuthHandler>("Demo", _ => {});

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder("Demo")
        .RequireAuthenticatedUser()
        .Build();
    options.FallbackPolicy = new AuthorizationPolicyBuilder("Demo")
        .RequireAuthenticatedUser()
        .Build();
});

await using var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/audits/{id}", (string id) =>
    Results.Ok(new { id, owner = "Municipal Water Quality Office", result = "Review pending" }));

app.MapGet("/api/audits/export", () =>
    Results.Ok(new { export = "quarterly-audit-summary", records = 12 }));

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .AllowAnonymous();

await app.StartAsync();
try
{
    var addresses = app.Services.GetRequiredService<IServer>()
        .Features.Get<IServerAddressesFeature>()?.Addresses;
    var address = addresses?.SingleOrDefault(x =>
        x.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        ?? throw new InvalidOperationException("Cannot discover loopback server address.");

    if (mode == "--serve")
    {
        Console.WriteLine($"Lab API running at {address}");
        await app.WaitForShutdownAsync();
        return 0;
    }

    using var client = new HttpClient { BaseAddress = new Uri(address) };
    using var anonymousAudit = await client.GetAsync("/api/audits/A-204");
    using var anonymousExport = await client.GetAsync("/api/audits/export");
    using var health = await client.GetAsync("/health");

    using var authenticatedRequest =
        new HttpRequestMessage(HttpMethod.Get, "/api/audits/A-204");
    authenticatedRequest.Headers.TryAddWithoutValidation("X-Demo-User", "auditor");
    using var authenticatedAudit = await client.SendAsync(authenticatedRequest);

    Console.WriteLine($"anonymous-audit={(int)anonymousAudit.StatusCode}");
    Console.WriteLine($"anonymous-export={(int)anonymousExport.StatusCode}");
    Console.WriteLine($"authenticated-audit={(int)authenticatedAudit.StatusCode}");
    Console.WriteLine($"anonymous-health={(int)health.StatusCode}");

    var commonContract =
        authenticatedAudit.StatusCode == HttpStatusCode.OK &&
        health.StatusCode == HttpStatusCode.OK;

    if (mode == "--reproduce")
    {
        var exposed = commonContract &&
            anonymousAudit.StatusCode == HttpStatusCode.OK &&
            anonymousExport.StatusCode == HttpStatusCode.OK;
        Console.WriteLine(exposed ? "REPRODUCED: sensitive anonymous reads succeeded." :
            "NOT REPRODUCED: compare statuses with the starter contract.");
        return exposed ? 0 : 1;
    }

    var secured = commonContract &&
        anonymousAudit.StatusCode == HttpStatusCode.Unauthorized &&
        anonymousExport.StatusCode == HttpStatusCode.Unauthorized;
    Console.WriteLine(secured ? "VERIFIED: protected and public routes satisfy contract." :
        "FAILED: learner-editable starter still violates at least one contract.");
    return secured ? 0 : 1;
}
finally
{
    await app.StopAsync();
}

sealed class DemoAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DemoAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Demo-User", out var user) ||
            string.IsNullOrWhiteSpace(user.ToString()))
            return Task.FromResult(AuthenticateResult.NoResult());

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, user.ToString()) }, Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
