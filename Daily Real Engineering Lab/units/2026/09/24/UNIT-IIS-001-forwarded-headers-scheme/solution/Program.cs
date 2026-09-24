using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Add(IPAddress.Loopback);
});

var app = builder.Build();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.MapGet("/portal", (HttpContext context) => Results.Ok(new { message = "portal-ok", scheme = context.Request.Scheme }));
app.Run();