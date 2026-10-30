using System.Text.Json;

namespace AirQualityLab;

internal sealed record PublishedReading(string StationId, decimal Pm25, string Unit);

internal static class MeasurementPublisher
{
    public static PublishedReading Publish(string json)
    {
        JsonElement measurement = MeasurementReader.ReadMeasurement(json);
        string stationId = measurement.GetProperty("stationId").GetString()
            ?? throw new JsonException("stationId must not be null");
        JsonElement metrics = measurement.GetProperty("metrics");
        decimal pm25 = metrics.GetProperty("pm25").GetDecimal();
        string unit = metrics.GetProperty("unit").GetString()
            ?? throw new JsonException("unit must not be null");
        return new PublishedReading(stationId, pm25, unit);
    }
}
