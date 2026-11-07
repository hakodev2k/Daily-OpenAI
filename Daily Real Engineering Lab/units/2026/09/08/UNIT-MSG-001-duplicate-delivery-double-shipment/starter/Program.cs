var deliveries = new[]
{
    new OrderPaidEvent("evt-20260908-001", "ORD-4821"),
    new OrderPaidEvent("evt-20260908-001", "ORD-4821")
};

var shipping = new FakeShippingGateway();
var consumer = new ShippingConsumer(shipping);

foreach (var message in deliveries)
{
    Console.WriteLine($"Delivery EventId={message.EventId} OrderId={message.OrderId}");
    await consumer.HandleAsync(message);
}

Console.WriteLine($"DeliveryCount={deliveries.Length}");
Console.WriteLine($"ReservationCalls={shipping.Reservations.Count}");
Console.WriteLine($"ReservationsForOrder={shipping.Reservations.Count(x => x.OrderId == \"ORD-4821\")}");

public sealed record OrderPaidEvent(string EventId, string OrderId);
public sealed record ShipmentReservation(string ReservationId, string OrderId);

public sealed class ShippingConsumer(FakeShippingGateway shipping)
{
    public async Task HandleAsync(OrderPaidEvent message)
    {
        await shipping.ReserveAsync(message.OrderId);
    }
}

public sealed class FakeShippingGateway
{
    public List<ShipmentReservation> Reservations { get; } = [];

    public Task ReserveAsync(string orderId)
    {
        Reservations.Add(new ShipmentReservation(Guid.NewGuid().ToString("N"), orderId));
        return Task.CompletedTask;
    }
}
