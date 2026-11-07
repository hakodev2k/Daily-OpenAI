using System.Text;
using Microsoft.AspNetCore.Http;

public sealed class RequestAuditMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Bounded demo payloads only. Production must enforce a body-size limit
        // and a PII-safe audit policy before capturing request content.
        context.Request.EnableBuffering(bufferThreshold: 16 * 1024,
            bufferLimit: 64 * 1024);
        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
        var captured = await reader.ReadToEndAsync();
        context.Items["auditPayload"] = captured;
        context.Request.Body.Position = 0;
        await next(context);
    }
}
