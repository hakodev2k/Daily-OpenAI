using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.ContentLength is > 0)
    {
        using var reader = new StreamReader(
            context.Request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var payload = await reader.ReadToEndAsync();
        Console.WriteLine($"AUDIT payload={payload}");
    }

    await next(context);
});

app.MapPost("/orders", async (HttpRequest request) =>
{
    try
    {
        var command = await JsonSerializer.DeserializeAsync<CreateOrderRequest>(
            request.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        if (command is null || string.IsNullOrWhiteSpace(command.Sku))
            return Results.BadRequest(new { error = "Missing order JSON" });

        return Results.Created($"/orders/{command.OrderId}", new { command.OrderId, command.Sku });
    }
    catch (JsonException)
    {
        return Results.BadRequest(new { error = "Invalid or unavailable order JSON" });
    }
});

app.Run("http://127.0.0.1:5081");

public sealed record CreateOrderRequest(Guid OrderId, string Sku);