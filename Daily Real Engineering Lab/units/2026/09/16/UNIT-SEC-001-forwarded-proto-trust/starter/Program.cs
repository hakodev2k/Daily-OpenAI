using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var forwarded = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
};
// Investigation note: which network peers are allowed to influence this metadata?
forwarded.KnownNetworks.Clear();
forwarded.KnownProxies.Clear();
app.UseForwardedHeaders(forwarded);

app.MapGet("/diagnostics/request", (HttpContext context) => Results.Json(new
{
    scheme = context.Request.Scheme,
    remoteIp = context.Connection.RemoteIpAddress?.ToString()
}));

app.Run("http://127.0.0.1:5088");