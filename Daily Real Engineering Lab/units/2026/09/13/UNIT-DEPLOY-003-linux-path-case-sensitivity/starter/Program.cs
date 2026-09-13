var files = new Dictionary<string, string>(StringComparer.Ordinal)
{
    ["templates/invoice.html"] = "invoice-template"
};

var configuredPath = "Templates/Invoice.html";

Console.WriteLine($"Artifact entry: {files.Keys.Single()}");
Console.WriteLine($"Configured path: {configuredPath}");

if (!files.TryGetValue(configuredPath, out var template))
{
    Console.WriteLine("FAIL: configured path did not match the deployed artifact entry.");
    return 1;
}

Console.WriteLine($"PASS: {template}");
return 0;
