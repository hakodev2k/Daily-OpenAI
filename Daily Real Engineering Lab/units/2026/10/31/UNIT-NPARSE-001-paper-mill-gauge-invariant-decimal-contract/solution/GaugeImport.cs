using System.Globalization;

namespace PaperMillGauge;

public sealed record GaugeReading(string RollId, decimal ThicknessMm);

public static class GaugeImport
{
    private const NumberStyles WireNumberStyle =
        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

    public static IReadOnlyList<GaugeReading> ParseLines(IEnumerable<string> lines)
    {
        var readings = new List<GaugeReading>();
        foreach (var line in lines)
        {
            var fields = line.Split(';');
            if (fields.Length != 2 || string.IsNullOrWhiteSpace(fields[0]))
                throw new FormatException("Invalid gauge record.");

            if (!decimal.TryParse(fields[1], WireNumberStyle,
                    CultureInfo.InvariantCulture, out var thickness))
                throw new FormatException("Invalid gauge thickness wire format.");

            readings.Add(new GaugeReading(fields[0], thickness));
        }
        return readings;
    }
}
