using System.Text.Json;

namespace FreightContracts;

public sealed record CanonicalManifest(
    string ManifestId, string PartnerId, decimal GrossWeightKg, string HazardClass);

public sealed record ManifestV2(
    string? ManifestId, string? PartnerId, decimal GrossWeightKg, string? HazardClass);

public static class ManifestIngress
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static CanonicalManifest Parse(string json)
    {
        var request = JsonSerializer.Deserialize<ManifestV2>(json, Options)
            ?? throw new InvalidDataException("Manifest body is required.");

        if (string.IsNullOrWhiteSpace(request.ManifestId) ||
            string.IsNullOrWhiteSpace(request.PartnerId) ||
            request.GrossWeightKg <= 0)
            throw new InvalidDataException("Manifest has missing or invalid required fields.");

        return new CanonicalManifest(request.ManifestId, request.PartnerId,
            request.GrossWeightKg, request.HazardClass ?? "NONE");
    }
}
