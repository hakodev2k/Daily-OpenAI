using System.Text.Json;

namespace RoamingUsage;

public sealed record UsageSummary(IReadOnlyList<long> AcceptedEventIds, int TotalUnits);

public static class UsageImporter
{
    public static UsageSummary Summarize(string partnerPayload)
    {
        using var document = JsonDocument.Parse(partnerPayload);
        if (document.RootElement.ValueKind != JsonValueKind.Array)
            throw new JsonException("Expected an array of roaming usage events.");

        var acceptedIds = new List<long>();
        var seen = new HashSet<long>();
        var totalUnits = 0;

        foreach (var item in document.RootElement.EnumerateArray())
        {
            // Event IDs are integer identities, not approximate measurements.
            // Read directly into Int64; malformed/fractional/out-of-range IDs fail closed.
            var eventId = item.GetProperty("eventId").GetInt64();
            var units = item.GetProperty("units").GetInt32();

            if (seen.Add(eventId))
            {
                acceptedIds.Add(eventId);
                totalUnits = checked(totalUnits + units);
            }
        }

        return new UsageSummary(acceptedIds, totalUnits);
    }
}
