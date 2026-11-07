using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;

var templates = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
{
    ["INVOICE"] = "invoice-v3"
};

const string incomingId = "invoice";
var found = templates.TryGetValue(incomingId, out var template);

Console.WriteLine($"culture={CultureInfo.CurrentCulture.Name}");
Console.WriteLine($"incoming={incomingId}");
Console.WriteLine($"lookup={(found ? "FOUND" : "MISS")}");
if (found) Console.WriteLine($"template={template}");
