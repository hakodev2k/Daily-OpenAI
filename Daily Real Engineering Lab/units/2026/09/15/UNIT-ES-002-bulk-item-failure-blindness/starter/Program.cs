using System.Text.Json;

var fixture = Path.Combine(AppContext.BaseDirectory, "bulk-response.json");
using var document = JsonDocument.Parse(File.ReadAllText(fixture));
var root = document.RootElement;

// Investigation note: the HTTP request itself completed successfully.
// Which response signals describe the outcome of the batch as a whole?
var httpStatus = 200;

if (httpStatus is >= 200 and < 300)
{
    Console.WriteLine("BATCH_OK");
    return 0;
}

Console.WriteLine("BATCH_FAILED");
return 1;
