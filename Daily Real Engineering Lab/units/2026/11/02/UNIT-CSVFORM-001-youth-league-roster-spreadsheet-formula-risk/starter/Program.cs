using System.Text;
using LeagueRoster;

var registrations = new[]
{
    new Registration("R101", "Linh", "North", "=HYPERLINK(\"https://example.invalid\",\"Join\")"),
    new Registration("R102", "Minh", "South", "+SUM(1,2)"),
    new Registration("R103", "An", "East", "-1+2"),
    new Registration("R104", "Binh", "West", "@SUM(1,2)"),
    new Registration("R105", "Trang", "North", "\t=2+2"),
    new Registration("R106", "Huy", "South", "  =1+1"),
    new Registration("R107", "Mai", "East", "Coach says \"ready\",\nawait check-in"),
    new Registration("R108", "Khanh", "West", "2026-11-02")
};
string mode = args.Length == 0 ? "report" : args[0].ToLowerInvariant();
string csv = RosterCsvExporter.Export(registrations);
List<string[]> records;
try { records = Contract.ReadCsv(csv); }
catch (Exception ex) { Console.Error.WriteLine(ex.Message); return 2; }
if (records.Count != 9 || records.Any(r => r.Length != 4))
{
    Console.Error.WriteLine("FAIL: CSV row or column count changed."); return 2;
}
int risky = records.Skip(1).Sum(r => r.Count(Contract.Risky));
Console.WriteLine($"Registrations=8; ParsedRows={records.Count - 1}; RiskyCells={risky}");
if (mode == "report") return 0;
if (mode == "reproduce")
{
    if (risky != 6) { Console.Error.WriteLine("Original symptom not reproduced."); return 1; }
    Console.WriteLine("REPRODUCED: six formula-like cells; no formulas executed."); return 0;
}
if (mode != "verify") { Console.Error.WriteLine("Use report, reproduce or verify."); return 2; }
if (!records[0].SequenceEqual(new[] { "Id", "Participant", "Team", "Comment" }))
{ Console.Error.WriteLine("FAIL: header changed."); return 1; }
var source = registrations.Select(r => new[] { r.Id, r.Participant, r.Team, r.Comment }).ToArray();
for (int i = 0; i < source.Length; i++)
for (int j = 0; j < 4; j++)
{
    string original = source[i][j];
    string expected = Contract.Risky(original) ? "'" + original : original;
    if (!string.Equals(records[i + 1][j], expected, StringComparison.Ordinal))
    { Console.Error.WriteLine($"FAIL: row {i + 1}, col {j + 1} contract mismatch."); return 1; }
}
if (risky != 0) { Console.Error.WriteLine("FAIL: unsafe spreadsheet cells remain."); return 1; }
Console.WriteLine("PASS: exact field roundtrip, eight records, multiline data and zero risky cells.");
return 0;

static class Contract
{
    public static bool Risky(string value)
    {
        if (value.Length == 0 || value[0] == '\'') return false;
        if (value[0] is '\t' or '\r' or '\n') return true;
        string trimmed = value.TrimStart(' ', '\t', '\r', '\n');
        return trimmed.Length > 0 && (trimmed[0] is '=' or '+' or '-' or '@');
    }

    public static List<string[]> ReadCsv(string csv)
    {
        var rows = new List<string[]>();
        var fields = new List<string>();
        var cell = new StringBuilder();
        bool inQuotes = false, afterQuote = false;
        for (int i = 0; i < csv.Length; i++)
        {
            char ch = csv[i];
            if (inQuotes)
            {
                if (ch == '"' && i + 1 < csv.Length && csv[i + 1] == '"')
                { cell.Append('"'); i++; }
                else if (ch == '"') { inQuotes = false; afterQuote = true; }
                else cell.Append(ch);
                continue;
            }
            if (ch == '"' && cell.Length == 0 && !afterQuote) { inQuotes = true; continue; }
            if (ch == ',') { fields.Add(cell.ToString()); cell.Clear(); afterQuote = false; continue; }
            if (ch == '\n')
            { fields.Add(cell.ToString()); cell.Clear(); rows.Add(fields.ToArray()); fields.Clear(); afterQuote = false; continue; }
            if (afterQuote || ch == '"') throw new FormatException("Malformed CSV quote.");
            cell.Append(ch);
        }
        if (inQuotes) throw new FormatException("Unclosed CSV quote.");
        if (cell.Length > 0 || fields.Count > 0)
        { fields.Add(cell.ToString()); rows.Add(fields.ToArray()); }
        return rows;
    }
}
