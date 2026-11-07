using Microsoft.AspNetCore.Http;
public sealed class RequestAuditMiddleware
{
    private readonly RequestDelegate _next;
    public RequestAuditMiddleware(RequestDelegate next) => _next = next;
    public async Task InvokeAsync(HttpContext context)
    {
        using var captured = new MemoryStream();
        await context.Request.Body.CopyToAsync(captured, context.RequestAborted);
        context.Items["audit.bytes"] = captured.Length;
        await _next(context);
    }
}
