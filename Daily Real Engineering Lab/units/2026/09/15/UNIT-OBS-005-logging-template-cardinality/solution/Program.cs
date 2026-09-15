var collector = new LogCollector();

for (var orderId = 1001; orderId <= 1020; orderId++)
{
    collector.Write(
        "Order {OrderId} loaded successfully",
        new Dictionary<string, object?> { ["OrderId"] = orderId });
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
        var rendered = messageTemplate;
        foreach (var pair in properties)
            rendered = rendered.Replace("{" + pair.Key + "}", Convert.ToString(pair.Value));
        Console.WriteLine(rendered);
    }
}
