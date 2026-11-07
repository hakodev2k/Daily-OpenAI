using System.Text.Json;
using System.Text.Json.Serialization;

namespace RailDepot;

public sealed class ReservationPayload
{
    public required int SchemaVersion { get; init; }
    public required string ReservationId { get; init; }
    public required string PartCode { get; init; }
    public required int ReservedUnits { get; init; }
}

public static class ReservationReader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public static ReservationPayload Read(string json)
    {
        var payload = JsonSerializer.Deserialize<ReservationPayload>(json, Options)
            ?? throw new JsonException("Reservation body is empty.");

        if (payload.SchemaVersion != 1)
            throw new JsonException("Unsupported reservation schema version.");
        if (string.IsNullOrWhiteSpace(payload.ReservationId) ||
            string.IsNullOrWhiteSpace(payload.PartCode))
            throw new JsonException("Reservation identity is required.");
        if (payload.ReservedUnits < 0)
            throw new JsonException("Reserved units must be non-negative.");

        return payload;
    }
}
