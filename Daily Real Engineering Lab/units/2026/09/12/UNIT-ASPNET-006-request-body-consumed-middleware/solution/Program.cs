using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();

    using var reader = new StreamReader(
        context.Request.Body,
        Encoding.UTF8,
        detectEncodingFromByteOrderMarks: false,
        leaveOpen: true);

    var rawBody = await reader.ReadToEndAsync();
    Console.WriteLine($"AUDIT BODY: {rawBody}");

    context.Request.Body.Position = 0;
    await next(context);
});

app.MapPost("/webhooks/order", (OrderWebhook webhook) =>
{
    Console.WriteLine($"HANDLER orderId={webhook.OrderId}");
    return Results.Ok(new { received = webhook.OrderId });
});

app.Run();

public sealed record OrderWebhook(string OrderId, string Status);
