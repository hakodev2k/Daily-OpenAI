using System;

var registry = new PrinterRegistry();
var service = new LabelService(registry);
var first = service.Render("BOOK-104");
registry.Update("KIOSK-B", "NEW");
var second = service.Render("BOOK-105");
Console.WriteLine($"first={first}");
Console.WriteLine($"second={second}");
Console.WriteLine($"registryVersion={registry.Version}");
Console.WriteLine($"activePrefix={registry.Current.Prefix}");
Console.WriteLine($"refreshObserved={second.Contains("NEW", StringComparison.Ordinal)}");

public sealed record PrinterConfig(string PrinterId, string Prefix);
public sealed class PrinterRegistry
{
    private PrinterConfig _current = new("KIOSK-A", "OLD");
    public int Version { get; private set; } = 1;
    public PrinterConfig Current => _current;
    public void Update(string id, string prefix)
    {
        _current = new PrinterConfig(id, prefix);
        Version++;
    }
}
public sealed class LabelService
{
    private readonly PrinterRegistry _registry;
    public LabelService(PrinterRegistry registry) => _registry = registry;
    public string Render(string accession)
    {
        var settings = _registry.Current;
        return $"{settings.PrinterId}:{settings.Prefix}:{accession}";
    }
}

