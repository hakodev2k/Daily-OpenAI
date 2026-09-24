using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Production topology note: the public request reaches a trusted reverse proxy first.
// Investigate which request properties the backend can safely derive from that boundary.

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    Console.WriteLine($"backend scheme={context.Request.Scheme} host={context.Request.Host} forwardedProto={context.Request.Headers["X-Forwarded-Proto"]}");
    await next();
});

app.MapGet("/portal", (HttpContext context) => Results.Ok(new
{
    message = "portal-ok",
    scheme = context.Request.Scheme
}));

app.Run();