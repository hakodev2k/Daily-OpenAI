using System.Globalization;

CultureInfo.CurrentCulture = new CultureInfo("tr-TR");

var cache = new Dictionary<string, string>();
cache["FILE-001"] = "manual.pdf";

var requestedSku = "file-001";
var normalized = requestedSku.ToUpper();

Console.WriteLine($"CULTURE={CultureInfo.CurrentCulture.Name}");
Console.WriteLine($"NORMALIZED={normalized}");
Console.WriteLine(cache.TryGetValue(normalized, out var value)
    ? $"LOOKUP=HIT:{value}"
    : "LOOKUP=MISS");
