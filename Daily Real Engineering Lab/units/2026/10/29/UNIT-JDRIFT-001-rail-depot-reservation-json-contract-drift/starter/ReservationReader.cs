using System.Text.Json;

namespace RailDepot;

public sealed class ReservationPayload
{
    public int SchemaVersion { get; set; }
    public string ReservationId { get; set; } = "";
    public string PartCode { get; set; } = "";
    public int ReservedUnits { get; set; }
}

public static class ReservationReader
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static ReservationPayload Read(string json)
    {
        return JsonSerializer.Deserialize<ReservationPayload>(json, Options)
            ?? throw new JsonException("Reservation body is empty.");
    }
}
