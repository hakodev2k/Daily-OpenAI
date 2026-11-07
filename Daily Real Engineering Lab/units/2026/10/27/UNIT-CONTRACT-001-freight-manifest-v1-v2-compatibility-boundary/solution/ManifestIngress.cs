using System.Text.Json;

namespace FreightContracts;

public sealed record CanonicalManifest(
    string ManifestId, string PartnerId, decimal GrossWeightKg, string HazardClass);

public static class ManifestIngress
{
    public static CanonicalManifest Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
            throw new InvalidDataException("Manifest must be a JSON object.");

        var version = 1;
        var hasVersion = root.TryGetProperty("schemaVersion", out var declaredVersion);
        if (hasVersion &&
            (declaredVersion.ValueKind != JsonValueKind.Number ||
             !declaredVersion.TryGetInt32(out version)))
            throw new InvalidDataException("schemaVersion must be an integer.");

        var id = RequiredString(root, "manifestId");
        var partner = RequiredString(root, "partnerId");
        return version switch
        {
            1 when !hasVersion && !root.TryGetProperty("grossWeightKg", out _) =>
                new CanonicalManifest(id, partner, PositiveDecimal(root, "weightKg"), "NOT_APPLICABLE"),
            2 when !root.TryGetProperty("weightKg", out _) =>
                new CanonicalManifest(id, partner, PositiveDecimal(root, "grossWeightKg"),
                    ValidHazard(RequiredString(root, "hazardClass"))),
            _ => throw new InvalidDataException("Unsupported or ambiguous manifest contract.")
        };
    }

    private static string RequiredString(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var value) ||
            value.ValueKind != JsonValueKind.String ||
            string.IsNullOrWhiteSpace(value.GetString()))
            throw new InvalidDataException($"Missing or invalid {name}.");
        return value.GetString()!;
    }

    private static decimal PositiveDecimal(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out var value) ||
            value.ValueKind != JsonValueKind.Number ||
            !value.TryGetDecimal(out var number) || number <= 0)
            throw new InvalidDataException($"Missing or invalid {name}.");
        return number;
    }

    private static string ValidHazard(string hazard) => hazard switch
    {
        "NONE" or "FLAMMABLE" or "CORROSIVE" => hazard,
        _ => throw new InvalidDataException("Unsupported hazardClass.")
    };
}
