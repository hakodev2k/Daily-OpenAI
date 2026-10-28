using System.Globalization;
using System.Text;
using GrantExports;

var mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
if (mode is not ("demo" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [demo|reproduce|verify]");
    return 2;
}

var fixtures = new List<GrantRow>
{
    new("B001", "Community, Inc.", 125.50m),
    new("B002", "Normal \"Quoted\" Name", 25m),
    new("B003", "=1+2", 10m),
    new("B004", "+SUM(1,2)", 11m),
    new("B005", "-2+3", 12m),
    new("B006", "@SUM(1,2)", 13m),
    new("B007", "\t=1+1", 14m),
    new("B008", " =1+1", 15m),
    new("B009", "Line\nBreak", 16m)
};

try
{
    var csv = GrantCsvExporter.Export(fixtures);
    var records = CsvProbe.Parse(csv);
    if (records.Count != fixtures.Count + 1)
        throw new InvalidOperationException("Incorrect CSV record count.");
    if (records[0].Length != 3 || records[0][0] != "BeneficiaryId" ||
        records[0][1] != "DisplayName" || records[0][2] != "Amount")
        throw new InvalidOperationException("Unexpected CSV header.");

    var riskyIds = new HashSet<string>(StringComparer.Ordinal)
        { "B003", "B004", "B005", "B006", "B007", "B008" };
    var unsafeCount = 0;
    for (var i = 0; i < fixtures.Count; i++)
    {
        var expected = fixtures[i];
        var actual = records[i + 1];
        if (actual.Length != 3 || actual[0] != expected.BeneficiaryId ||
            actual[2] != expected.Amount.ToString("0.00", CultureInfo.InvariantCulture))
            throw new InvalidOperationException($"CSV structure/amount mismatch at {expected.BeneficiaryId}.");

        var name = actual[1];
        if (riskyIds.Contains(expected.BeneficiaryId))
        {
            if (CsvProbe.IsFormulaCandidate(name)) unsafeCount++;
            if (mode == "verify" &&
                (CsvProbe.IsFormulaCandidate(name) || name == expected.DisplayName ||
                 !name.EndsWith(expected.DisplayName, StringComparison.Ordinal) ||
                 name.Length > expected.DisplayName.Length + 2))
                throw new InvalidOperationException($"Untrusted text contract failed at {expected.BeneficiaryId}.");
        }
        else if (name != expected.DisplayName)
        {
            throw new InvalidOperationException($"Ordinary text changed at {expected.BeneficiaryId}.");
        }
    }

    Console.WriteLine($"Rows={fixtures.Count}; formula-like parsed text cells={unsafeCount}; mode={mode}");
    if (mode == "demo") { Console.WriteLine(csv); return 0; }
    if (mode == "reproduce" && unsafeCount != riskyIds.Count)
        throw new InvalidOperationException("Baseline symptom no longer reproduces.");
    if (mode == "verify" && unsafeCount != 0)
        throw new InvalidOperationException("Untrusted cells remain formula-like after CSV parsing.");
    Console.WriteLine(mode == "verify" ? "PASS: learner-edited exporter verified." : "PASS: baseline reproduced.");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"FAIL: {ex.Message}");
    return 1;
}

internal static class CsvProbe
{
    public static bool IsFormulaCandidate(string value)
    {
        var visible = value.TrimStart(' ', '\t', '\r', '\n');
        return visible.Length > 0 && "=+-@".Contains(visible[0]);
    }

    public static List<string[]> Parse(string csv)
    {
        var result = new List<string[]>();
        var row = new List<string>();
        var cell = new StringBuilder();
        var inQuotes = false;
        var closedQuote = false;

        void FinishCell()
        {
            row.Add(cell.ToString());
            cell.Clear();
            closedQuote = false;
        }

        void FinishRow()
        {
            FinishCell();
            result.Add(row.ToArray());
            row = new List<string>();
        }

        for (var i = 0; i < csv.Length; i++)
        {
            var c = csv[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < csv.Length && csv[i + 1] == '"')
                    {
                        cell.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                        closedQuote = true;
                    }
                }
                else cell.Append(c);
            }
            else if (c == '"')
            {
                if (cell.Length != 0 || closedQuote)
                    throw new FormatException("Unexpected CSV quote.");
                inQuotes = true;
            }
            else if (c == ',') FinishCell();
            else if (c is '\r' or '\n')
            {
                if (c == '\r' && i + 1 < csv.Length && csv[i + 1] == '\n') i++;
                FinishRow();
            }
            else
            {
                if (closedQuote) throw new FormatException("Unexpected text after closing quote.");
                cell.Append(c);
            }
        }
        if (inQuotes) throw new FormatException("Unterminated CSV field.");
        if (cell.Length > 0 || row.Count > 0 || closedQuote) FinishRow();
        return result;
    }
}
