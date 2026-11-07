using System.Globalization;
using System.Text;

namespace GrantExports;

public sealed record GrantRow(string BeneficiaryId, string DisplayName, decimal Amount);

public static class GrantCsvExporter
{
    public static string Export(IEnumerable<GrantRow> rows)
    {
        var csv = new StringBuilder("BeneficiaryId,DisplayName,Amount\r\n");
        foreach (var row in rows)
        {
            csv.Append(EscapeCsv(row.BeneficiaryId)).Append(',')
               .Append(EscapeCsv(AsSpreadsheetText(row.DisplayName))).Append(',')
               .Append(row.Amount.ToString("0.00", CultureInfo.InvariantCulture))
               .Append("\r\n");
        }
        return csv.ToString();
    }

    private static string AsSpreadsheetText(string value)
    {
        var meaningful = value.TrimStart(' ', '\t', '\r', '\n');
        if (meaningful.Length == 0 || !"=+-@".Contains(meaningful[0]))
            return value;
        // Local CSV policy, not a universal cross-client guarantee.
        return "'" + value;
    }

    private static string EscapeCsv(string value)
    {
        if (value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0)
            return value;
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}
