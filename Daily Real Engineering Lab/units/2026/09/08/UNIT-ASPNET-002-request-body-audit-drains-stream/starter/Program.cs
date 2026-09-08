using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/webhooks"))
    {
        using var reader = new StreamReader(
            context.Request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var rawBody = await reader.ReadToEndAsync();
        Console.WriteLine($"AUDIT_BODY={rawBody}");
    }

    await next();
});

app.MapPost("/webhooks/payment", async (HttpRequest request) =>
{
    var payload = await JsonSerializer.DeserializeAsync<PaymentWebhook>(
        request.Body,
        new JsonSerializerOptions(JsonSerializerDefaults.Web));

    if (payload is null || string.IsNullOrWhiteSpace(payload.PaymentId))
    {
        return Results.BadRequest(new { error = "invalid payload" });
    }

    return Results.Ok(new { payload.PaymentId, payload.Amount });
});

app.Run("http://127.0.0.1:5086");

public sealed record PaymentWebhook(string PaymentId, decimal Amount);
