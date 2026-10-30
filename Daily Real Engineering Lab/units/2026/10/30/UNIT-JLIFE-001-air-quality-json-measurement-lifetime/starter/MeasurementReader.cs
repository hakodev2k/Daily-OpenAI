using System.Text.Json;

namespace AirQualityLab;

internal static class MeasurementReader
{
    public static JsonElement ReadMeasurement(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.GetProperty("measurement");
    }
}
