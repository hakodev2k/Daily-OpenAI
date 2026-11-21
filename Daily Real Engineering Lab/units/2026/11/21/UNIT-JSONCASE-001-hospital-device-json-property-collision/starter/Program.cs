using System.Text.Json;
using System.Text.Json.Serialization;

if (args.Contains("--contract-check"))
{
    Environment.ExitCode = ContractChecks.Run(Parse);
    return;
}

foreach (var payload in Samples.All)
{
    try
    {
        var reading = Parse(payload);
        Console.WriteLine($"INPUT={payload} | DEVICE={reading.DeviceId} | TEMP={reading.TemperatureC} | ALERT={reading.TemperatureC >= 8}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"INPUT={payload} | REJECTED={ex.GetType().Name}: {ex.Message}");
    }
}

static Reading Parse(string payload)
{
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.Strict
    };
    return JsonSerializer.Deserialize<Reading>(payload, options)
        ?? throw new JsonException("Empty reading");
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
