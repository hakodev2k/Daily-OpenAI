using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

var orders = new Dictionary<int, OrderStatus>
{
    [42] = OrderStatus.Paid
};

app.MapPut("/orders/{id:int}/status", (int id, UpdateStatusRequest request) =>
{
    if (!orders.ContainsKey(id))
    {
        return Results.NotFound();
    }

    // Investigation note:
    // The request reached a strongly typed enum. Is that alone sufficient
    // evidence that the value belongs to the public domain contract?
    orders[id] = request.Status;

    return Results.Ok(new
    {
        orderId = id,
        status = orders[id],
        numericStatus = (int)orders[id]
    });
});

app.MapGet("/orders/{id:int}", (int id) =>
    orders.TryGetValue(id, out var status)
        ? Results.Ok(new { orderId = id, status, numericStatus = (int)status })
        : Results.NotFound());

app.Run();

public enum OrderStatus
{
    Pending = 0,
    Paid = 1,
    Shipped = 2,
    Cancelled = 3
}

public sealed record UpdateStatusRequest(OrderStatus Status);