using System.Text.Json;

namespace MuseumConditionLab;

public sealed record ConditionReport(string ExhibitId, int HumidityPercent, string Status);

public static class ConditionReportExporter
{
    private static readonly JsonSerializerOptions SharedOptions = new(JsonSerializerDefaults.Web);

    public static string Export(ConditionReport report, bool pretty)
    {
        SharedOptions.WriteIndented = pretty;
        return JsonSerializer.Serialize(report, SharedOptions);
    }
}
