using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Hợp đồng lab: edge TLS termination gửi request HTTP từ 127.0.0.1.
// Trong production phải cấu hình địa chỉ proxy thực, không tin mọi nguồn.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    options.KnownProxies.Add(IPAddress.Loopback);
    options.ForwardLimit = 1;
});

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443;
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
});

var app = builder.Build();

// Hãy quan sát scheme tại mỗi giai đoạn và lý do một request dừng pipeline.
app.UseForwardedHeaders();
app.UseHttpsRedirection();

app.MapGet("/status", (HttpContext context) =>
    Results.Json(new
    {
        service = "partner-shipment-api",
        scheme = context.Request.Scheme,
        requestId = context.TraceIdentifier
    }));

app.Run();
