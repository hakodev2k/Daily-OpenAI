using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false));
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

    orders[id] = request.Status;
    return Results.Ok(new { orderId = id, status = orders[id], numericStatus = (int)orders[id] });
});

app.Run();

public enum OrderStatus
{
    Pending = 0,
    Paid = 1,
    Shipped = 2,
    Cancelled = 3
}

public sealed record UpdateStatusRequest(OrderStatus Status);