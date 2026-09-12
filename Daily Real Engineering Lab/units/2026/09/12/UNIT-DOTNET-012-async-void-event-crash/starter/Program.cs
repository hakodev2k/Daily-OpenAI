var publisher = new OrderStatusPublisher();
publisher.StatusChanged += SendNotificationAsync;

try
{
    publisher.Publish("ORD-42");
    Console.WriteLine("Publish completed");
}
catch (Exception ex)
{
    Console.WriteLine($"Publisher caught: {ex.Message}");
}

await Task.Delay(300);
Console.WriteLine("Process finished normally");

static async void SendNotificationAsync(string orderId)
{
    await Task.Delay(100);
    Console.WriteLine($"Notification failed for {orderId}");
    throw new InvalidOperationException("notification provider unavailable");
}

sealed class OrderStatusPublisher
{
    public event Action<string>? StatusChanged;
    public void Publish(string orderId) => StatusChanged?.Invoke(orderId);
}
