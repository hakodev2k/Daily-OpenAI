using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication("Header")
    .AddScheme<AuthenticationSchemeOptions, HeaderAuthenticationHandler>("Header", _ => { });

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/secure") &&
        !(context.User.Identity?.IsAuthenticated ?? false))
    {
        app.Logger.LogWarning("Access gate rejected request. Authenticated={Authenticated}", context.User.Identity?.IsAuthenticated);
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized");
        return;
    }

    await next();
});

app.UseAuthentication();

app.MapGet("/public", () => Results.Ok(new { status = "ok" }));
app.MapGet("/secure", (HttpContext context) =>
    Results.Ok(new { user = context.User.Identity?.Name, authenticated = context.User.Identity?.IsAuthenticated }));

app.Run();

sealed class HeaderAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public HeaderAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-User", out var value) || string.IsNullOrWhiteSpace(value))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, value.ToString()) },
            Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
