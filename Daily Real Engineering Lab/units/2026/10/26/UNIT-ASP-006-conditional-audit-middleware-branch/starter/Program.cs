using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

// In-memory host: the real ASP.NET Core pipeline, no network port required.
var services = new ServiceCollection().BuildServiceProvider();
var app = new ApplicationBuilder(services);

app.MapWhen(
    context => context.Request.Path.StartsWithSegments("/api"),
    branch => branch.Use(async (context, next) =>
    {
        context.Response.Headers["X-Audit-Policy"] = "audit-v1";
        await next(context);
    }));

app.Run(async context =>
{
    var payload = context.Request.Path.Value switch
    {
        "/health" => "healthy",
        "/api/v1/maintenance/active" => "active:3",
        "/api/v1/maintenance/history" => "history:2",
        _ => null
    };
    if (payload is null)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }
    context.Response.StatusCode = StatusCodes.Status200OK;
    await context.Response.WriteAsync(payload);
});

var pipeline = app.Build();
var health = await ProbeAsync(pipeline, "/health");
var active = await ProbeAsync(pipeline, "/api/v1/maintenance/active");
var history = await ProbeAsync(pipeline, "/api/v1/maintenance/history");
var missing = await ProbeAsync(pipeline, "/missing");

foreach (var result in new[] { health, active, history, missing })
    Console.WriteLine($"{result.Path}: status={result.Status}, body='{result.Body}', audit='{result.AuditPolicy}'");

var baseContract = health.Status == 200 && health.Body == "healthy" && health.AuditPolicy == "" &&
                   missing.Status == 404 && missing.AuditPolicy == "";
var reproduced = baseContract && active.Status == 404 && history.Status == 404 &&
                 active.AuditPolicy == "audit-v1" && history.AuditPolicy == "audit-v1";
var verified = baseContract &&
               active.Status == 200 && active.Body == "active:3" && active.AuditPolicy == "audit-v1" &&
               history.Status == 200 && history.Body == "history:2" && history.AuditPolicy == "audit-v1";
var mode = args.Length > 0 ? args[0].TrimStart('-').ToLowerInvariant() : "inspect";
switch (mode)
{
    case "reproduce":
        Console.WriteLine(reproduced ? "REPRODUCED: API routes fail while audit policy is applied." :
            "NOT REPRODUCED: expected starter symptom was not observed.");
        return reproduced ? 0 : 1;
    case "verify":
        Console.WriteLine(verified ? "PASS: API and audit boundaries meet the contract." :
            "FAIL: API behavior or audit boundaries violate the contract.");
        return verified ? 0 : 1;
    case "inspect": return 0;
    default:
        Console.Error.WriteLine("Usage: inspect | reproduce | verify");
        return 2;
}

static async Task<Probe> ProbeAsync(RequestDelegate pipeline, string path)
{
    var context = new DefaultHttpContext();
    context.Request.Path = path;
    context.Response.Body = new MemoryStream();
    await pipeline(context);
    var body = Encoding.UTF8.GetString(((MemoryStream)context.Response.Body).ToArray());
    return new Probe(path, context.Response.StatusCode, body,
        context.Response.Headers["X-Audit-Policy"].ToString());
}

internal readonly record struct Probe(string Path, int Status, string Body, string AuditPolicy);
