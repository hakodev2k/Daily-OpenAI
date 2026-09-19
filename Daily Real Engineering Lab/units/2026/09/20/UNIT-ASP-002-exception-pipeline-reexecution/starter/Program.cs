using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AuditSink>();
var app = builder.Build();

app.UseExceptionHandler("/error");

app.Use(async (context, next) =>
{
    var sink = context.RequestServices.GetRequiredService<AuditSink>();
    var requestId = context.Request.Headers["X-Lab-Request-Id"].FirstOrDefault() ?? context.TraceIdentifier;

    await next();

    // Investigation note:
    // Does this middleware execute exactly once for every logical client request
    // on every path through the application?
    sink.Add(requestId, context.Request.Path);
});

app.MapGet("/tickets/ok", () => Results.Ok(new { status = "ok" }));
app.MapGet("/tickets/fail", () => throw new InvalidOperationException("Simulated ticket processing failure"));
app.MapGet("/error", (HttpContext context) =>
{
    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
    return Results.Problem(title: "Ticket processing failed", detail: feature?.Error.Message, statusCode: 500);
});
app.MapGet("/__lab/audits/{requestId}", (string requestId, AuditSink sink) => sink.Count(requestId));
app.MapDelete("/__lab/audits", (AuditSink sink) => { sink.Clear(); return Results.NoContent(); });

app.Run();

public sealed class AuditSink
{
    private readonly object _gate = new();
    private readonly List<(string RequestId, string Path)> _entries = new();

    public void Add(string requestId, string path)
    {
        lock (_gate) _entries.Add((requestId, path));
    }

    public int Count(string requestId)
    {
        lock (_gate) return _entries.Count(x => x.RequestId == requestId);
    }

    public void Clear()
    {
        lock (_gate) _entries.Clear();
    }
}
