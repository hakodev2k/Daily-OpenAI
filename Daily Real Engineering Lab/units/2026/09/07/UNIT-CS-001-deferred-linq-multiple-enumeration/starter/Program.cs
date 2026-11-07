var orders = new[]
{
    new Order("ORD-001", "SKU-RED"),
    new Order("ORD-002", "SKU-BLUE"),
    new Order("ORD-003", "SKU-GREEN")
};

var inventory = new InventoryClient(["SKU-RED", "SKU-GREEN"]);

IEnumerable<Order> eligibleOrders =
    orders.Where(order => inventory.CanReserve(order.Sku));

Console.WriteLine($"Eligible orders: {eligibleOrders.Count()}");

foreach (var order in eligibleOrders)
{
    Console.WriteLine($"Reserve {order.Id}");
}

Console.WriteLine($"Inventory calls: {inventory.CallCount}");

internal sealed record Order(string Id, string Sku);

internal sealed class InventoryClient
{
    private readonly HashSet<string> _reservableSkus;

    public InventoryClient(IEnumerable<string> reservableSkus)
        => _reservableSkus = reservableSkus.ToHashSet(StringComparer.Ordinal);

    public int CallCount { get; private set; }

    public bool CanReserve(string sku)
    {
        CallCount++;
        return _reservableSkus.Contains(sku);
    }
}
