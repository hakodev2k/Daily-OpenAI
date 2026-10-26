using System.Text.Json;

namespace MuseumConditionLab;

public sealed record ConditionReport(string ExhibitId, int HumidityPercent, string Status);

public static class ConditionReportExporter
{
    private static readonly JsonSerializerOptions CompactOptions =
        new(JsonSerializerDefaults.Web) { WriteIndented = false };
    private static readonly JsonSerializerOptions PrettyOptions =
        new(JsonSerializerDefaults.Web) { WriteIndented = true };

    public static string Export(ConditionReport report, bool pretty)
    {
        ArgumentNullException.ThrowIfNull(report);
        return JsonSerializer.Serialize(report, pretty ? PrettyOptions : CompactOptions);
    }
}
