using System.Globalization;

CultureInfo.CurrentCulture = new CultureInfo("tr-TR");

var cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    ["FILE-001"] = "manual.pdf"
};

var requestedSku = "file-001";

Console.WriteLine($"CULTURE={CultureInfo.CurrentCulture.Name}");
Console.WriteLine(cache.TryGetValue(requestedSku, out var value)
    ? $"LOOKUP=HIT:{value}"
    : "LOOKUP=MISS");
