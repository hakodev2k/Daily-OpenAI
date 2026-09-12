using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    // Investigation note:
    // This middleware needs the raw payload for audit/signature diagnostics.
    // Consider what downstream components observe after this read finishes.
    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
    var rawBody = await reader.ReadToEndAsync();
    Console.WriteLine($"AUDIT BODY: {rawBody}");

    await next(context);
});

app.MapPost("/webhooks/order", (OrderWebhook webhook) =>
{
    Console.WriteLine($"HANDLER orderId={webhook.OrderId}");
    return Results.Ok(new { received = webhook.OrderId });
});

app.Run();

public sealed record OrderWebhook(string OrderId, string Status);
