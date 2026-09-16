using System.Text.Json;

var record = new CustomerRecord(42, "CUST-42", "Lan Nguyen");
var json = JsonSerializer.Serialize(record, new JsonSerializerOptions(JsonSerializerDefaults.Web));

Console.WriteLine(json);

const string expected = "{\"id\":42,\"customerCode\":\"CUST-42\",\"displayName\":\"Lan Nguyen\"}";
if (json != expected)
{
    Console.Error.WriteLine("CONTRACT_REGRESSION: response JSON no longer matches the published client contract.");
    Environment.ExitCode = 2;
}
else
{
    Console.WriteLine("CONTRACT_OK");
}

// Persistence naming was changed during a storage-model cleanup.
// Investigation note: which type should be allowed to define the public response shape?
public sealed record CustomerRecord(int Id, string ExternalCode, string FullName);