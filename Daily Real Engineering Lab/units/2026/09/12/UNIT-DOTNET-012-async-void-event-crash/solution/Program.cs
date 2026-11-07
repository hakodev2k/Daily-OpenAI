var publisher = new OrderStatusPublisher();
publisher.Subscribe(SendNotificationAsync);

try
{
    await publisher.PublishAsync("ORD-42");
    Console.WriteLine("Publish completed");
}
catch (Exception ex)
{
    Console.WriteLine($"Publisher caught: {ex.Message}");
}

Console.WriteLine("Process finished normally");

static async Task SendNotificationAsync(string orderId)
{
    await Task.Delay(100);
    Console.WriteLine($"Notification failed for {orderId}");
    throw new InvalidOperationException("notification provider unavailable");
}

sealed class OrderStatusPublisher
{
    private readonly List<Func<string, Task>> _handlers = [];
    public void Subscribe(Func<string, Task> handler) => _handlers.Add(handler);
    public Task PublishAsync(string orderId) => Task.WhenAll(_handlers.Select(h => h(orderId)));
}
