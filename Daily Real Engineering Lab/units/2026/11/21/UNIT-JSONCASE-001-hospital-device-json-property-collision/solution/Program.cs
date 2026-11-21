using System.Text.Json;
using System.Text.Json.Serialization;

if (args.Contains("--contract-check")) { Environment.ExitCode = ContractChecks.Run(Parse); return; }
foreach (var payload in Samples.All)
{
    try { var r = Parse(payload); Console.WriteLine($"DEVICE={r.DeviceId} TEMP={r.TemperatureC} ALERT={r.TemperatureC >= 8}"); }
    catch (JsonException e) { Console.WriteLine($"REJECTED={e.Message}"); }
}
static Reading Parse(string payload)
{
    using var doc = JsonDocument.Parse(payload);
    if (doc.RootElement.ValueKind != JsonValueKind.Object) throw new JsonException("Object required");
    var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (var p in doc.RootElement.EnumerateObject())
        if (!keys.Add(p.Name)) throw new JsonException("Ambiguous property name");
    if (!keys.Contains("DeviceId") || !keys.Contains("TemperatureC")) throw new JsonException("Required field missing");
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, NumberHandling = JsonNumberHandling.Strict };
    var reading = JsonSerializer.Deserialize<Reading>(payload, options) ?? throw new JsonException("Empty reading");
    if (string.IsNullOrWhiteSpace(reading.DeviceId)) throw new JsonException("Device ID missing");
    return reading;
}
public sealed record Reading(string DeviceId, decimal TemperatureC);
public static class Samples
{
    public static readonly string[] All =
    [
        """{"DeviceId":"MED-17","TemperatureC":11.5}""",
        """{"deviceId":"MED-17","temperatureC":3.0}""",
        """{"DeviceId":"MED-17","TemperatureC":11.5,"temperatureC":3.0}"""
    ];
}
