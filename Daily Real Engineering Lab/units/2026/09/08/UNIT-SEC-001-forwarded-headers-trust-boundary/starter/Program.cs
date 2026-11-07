using System.Net;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/whoami", (HttpContext context) =>
{
    var clientIp = ResolveClientIp(context);
    return Results.Json(new
    {
        clientIp,
        internalRequest = IsInternal(clientIp)
    });
});

app.MapPost("/ops/cache/clear", (HttpContext context) =>
{
    var clientIp = ResolveClientIp(context);
    return IsInternal(clientIp)
        ? Results.Ok(new { cleared = true, clientIp })
        : Results.StatusCode(StatusCodes.Status403Forbidden);
});

app.Run("http://127.0.0.1:5074");

static string ResolveClientIp(HttpContext context)
{
    if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
    {
        var first = forwarded.ToString().Split(',')[0].Trim();
        if (!string.IsNullOrWhiteSpace(first))
        {
            return first;
        }
    }

    return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}

static bool IsInternal(string clientIp)
{
    if (!IPAddress.TryParse(clientIp, out var ip)) return false;
    var bytes = ip.GetAddressBytes();
    return bytes.Length == 4 && bytes[0] == 10;
}
