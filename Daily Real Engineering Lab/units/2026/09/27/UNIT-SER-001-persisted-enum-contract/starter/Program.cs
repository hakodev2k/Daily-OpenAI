using System.Text.Json;

const string historicalJson = "{\"OrderId\":42,\"Status\":1}";
const OrderStatus expectedHistoricalMeaning = OrderStatus.Paid;

Console.WriteLine($"RAW {historicalJson}");
var snapshot = JsonSerializer.Deserialize<OrderSnapshot>(historicalJson)
               ?? throw new InvalidOperationException("Snapshot was null.");

Console.WriteLine($"DESERIALIZED order={snapshot.OrderId} status={snapshot.Status} numeric={(int)snapshot.Status}");
Console.WriteLine($"EXPECTED status={expectedHistoricalMeaning}");

if (snapshot.Status != expectedHistoricalMeaning)
{
    Console.WriteLine("CONTRACT_MISMATCH");
    Environment.ExitCode = 2;
}
else
{
    Console.WriteLine("CONTRACT_OK");
}

public sealed record OrderSnapshot(int OrderId, OrderStatus Status);

public enum OrderStatus
{
    New,
    Pending,
    Paid,
    Shipped
}
