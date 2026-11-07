using Microsoft.AspNetCore.Http;
public sealed class RequestAuditMiddleware
{
    private readonly RequestDelegate _next;
    public RequestAuditMiddleware(RequestDelegate next) => _next = next;
    public async Task InvokeAsync(HttpContext context)
    {
        // Real requests can have forward-only streams; enable bounded replay.
        context.Request.EnableBuffering(bufferThreshold: 30 * 1024, bufferLimit: 1024 * 1024);
        using var captured = new MemoryStream();
        await context.Request.Body.CopyToAsync(captured, context.RequestAborted);
        context.Items["audit.bytes"] = captured.Length;
        context.Request.Body.Position = 0;
        await _next(context);
    }
}
