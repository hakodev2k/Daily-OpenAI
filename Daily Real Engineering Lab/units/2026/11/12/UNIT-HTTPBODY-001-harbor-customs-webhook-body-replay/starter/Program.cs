using System.Text.Json;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/webhooks/customs")
    {
        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var raw = await reader.ReadToEndAsync();
        context.Items["audit-length"] = raw.Length;
    }
    await next(context);
});
app.MapPost("/webhooks/customs", async (HttpContext context) =>
{
    try
    {
        var data = await JsonSerializer.DeserializeAsync<Shipment>(context.Request.Body);
        if (data is null || string.IsNullOrWhiteSpace(data.Reference))
            return Results.BadRequest("Missing reference");
        return Results.Ok(new { data.Reference, AuditLength = context.Items["audit-length"] });
    }
    catch (JsonException) { return Results.BadRequest("Invalid JSON"); }
});
app.Run();
public record Shipment(string Reference);
