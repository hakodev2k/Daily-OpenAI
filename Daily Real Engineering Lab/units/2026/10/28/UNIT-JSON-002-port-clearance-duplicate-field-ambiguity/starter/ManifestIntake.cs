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
        var declaration = JsonSerializer.Deserialize<ClearanceDeclaration>(
            payload, PartnerOptions)
            ?? throw new JsonException("Declaration cannot be null.");

        if (string.IsNullOrWhiteSpace(declaration.VesselId) ||
            string.IsNullOrWhiteSpace(declaration.CargoCode))
        {
            throw new JsonException("VesselId and CargoCode are required.");
        }

        return declaration;
    }
}
