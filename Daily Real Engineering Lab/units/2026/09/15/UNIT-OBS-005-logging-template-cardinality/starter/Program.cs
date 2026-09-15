var collector = new LogCollector();

for (var orderId = 1001; orderId <= 1020; orderId++)
{
    var template = $"Order {orderId} loaded successfully";
    collector.Write(template, new Dictionary<string, object?>());
}

Console.WriteLine($"EVENTS={collector.EventCount}");
Console.WriteLine($"TEMPLATES={collector.TemplateCount}");

public sealed class LogCollector
{
    private readonly HashSet<string> _templates = new(StringComparer.Ordinal);
    public int EventCount { get; private set; }
    public int TemplateCount => _templates.Count;

    public void Write(string messageTemplate, IReadOnlyDictionary<string, object?> properties)
    {
        EventCount++;
        _templates.Add(messageTemplate);
        Console.WriteLine(messageTemplate);
    }
}
