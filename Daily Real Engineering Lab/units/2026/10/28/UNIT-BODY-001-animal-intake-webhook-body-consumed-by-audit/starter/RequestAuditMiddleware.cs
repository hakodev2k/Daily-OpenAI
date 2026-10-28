using System.Text;
using Microsoft.AspNetCore.Http;

public sealed class RequestAuditMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
        var captured = await reader.ReadToEndAsync();
        context.Items["auditPayload"] = captured;
        await next(context);
    }
}
