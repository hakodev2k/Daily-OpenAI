using System.Globalization;

namespace PaperMillGauge;

public sealed record GaugeReading(string RollId, decimal ThicknessMm);

public static class GaugeImport
{
    public static IReadOnlyList<GaugeReading> ParseLines(IEnumerable<string> lines)
    {
        var readings = new List<GaugeReading>();
        foreach (var line in lines)
        {
            var fields = line.Split(';');
            if (fields.Length != 2 || string.IsNullOrWhiteSpace(fields[0]))
                throw new FormatException("Invalid gauge record.");

            var thickness = decimal.Parse(
                fields[1], NumberStyles.Number, CultureInfo.CurrentCulture);
            readings.Add(new GaugeReading(fields[0], thickness));
        }
        return readings;
    }
}
