using System.Text.Json;

var fixture = Path.Combine(AppContext.BaseDirectory, "bulk-response.json");
using var document = JsonDocument.Parse(File.ReadAllText(fixture));
var failed = new List<string>();

foreach (var item in document.RootElement.GetProperty("items").EnumerateArray())
{
    var operation = item.EnumerateObject().Single().Value;
    var status = operation.GetProperty("status").GetInt32();
    if (status >= 300)
        failed.Add(operation.GetProperty("_id").GetString() ?? "<unknown>");
}

if (failed.Count == 0)
{
    Console.WriteLine("BATCH_OK");
    return 0;
}

Console.WriteLine($"FAILED_ITEMS={failed.Count}: {string.Join(",", failed)}");
return 1;
