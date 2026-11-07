using System.Text.Json;

namespace PortManifestLab;

public sealed record ClearanceDeclaration(
    string VesselId,
    string CargoCode,
    bool RequiresInspection);

public static class ManifestIntake
{
    private static readonly JsonSerializerOptions PartnerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static ClearanceDeclaration Parse(string payload)
    {
        using var document = JsonDocument.Parse(payload);

        if (document.RootElement.ValueKind != JsonValueKind.Object)
            throw new JsonException("Declaration must be a JSON object.");

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (!seen.Add(property.Name))
                throw new JsonException("Conflicting top-level JSON property names.");
        }

        var declaration = document.RootElement.Deserialize<ClearanceDeclaration>(
            PartnerOptions)
            ?? throw new JsonException("Declaration cannot be null.");

        if (string.IsNullOrWhiteSpace(declaration.VesselId) ||
            string.IsNullOrWhiteSpace(declaration.CargoCode))
        {
            throw new JsonException("VesselId and CargoCode are required.");
        }

        return declaration;
    }
}
