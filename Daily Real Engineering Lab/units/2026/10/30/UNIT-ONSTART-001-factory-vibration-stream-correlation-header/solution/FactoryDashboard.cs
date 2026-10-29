using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EngineeringLab;

public static class FactoryDashboard
{
    public static WebApplication Build()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Logging.ClearProviders();
        var app = builder.Build();

        app.Use(async (context, next) =>
        {
            string requestId = context.TraceIdentifier;
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Trace-Id"] = requestId;
                return Task.CompletedTask;
            });

            await next();
        });

        app.MapGet("/health", (HttpContext context) =>
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return Task.CompletedTask;
        });

        app.MapGet("/api/readings/live", async (HttpContext context) =>
        {
            context.Response.ContentType = "text/event-stream";
            await context.Response.WriteAsync("event: vibration\ndata: 2.4\n\n");
            await context.Response.Body.FlushAsync();
        });

        return app;
    }
}
