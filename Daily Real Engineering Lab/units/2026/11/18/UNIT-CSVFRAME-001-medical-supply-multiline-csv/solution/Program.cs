using System.Text;
using System.Linq;

var csv = "Sku,Quantity,Note\r\nGLOVE-01,12,\"Kho tầng 2\r\nkiểm kê lại\"\r\nMASK-02,8,\"Ưu tiên \"\"khoa cấp cứu\"\"\"\r\n";
var rows = Importer.Parse(csv);
if (args.Contains("--check"))
{
    var valid = rows.Count == 2 && rows[0].Sku == "GLOVE-01" && rows[0].Quantity == 12
        && rows[0].Note == "Kho tầng 2\r\nkiểm kê lại" && rows[1].Sku == "MASK-02"
        && rows[1].Quantity == 8 && rows[1].Note == "Ưu tiên \"khoa cấp cứu\"";
    var extra = Importer.Parse("Sku,Quantity,Note\nA,1,\"a,b\"\nB,2,\"x\"\n");
    var invalid = false;
    try { Importer.Parse("Sku,Quantity,Note\nC,1,\"unfinished"); }
    catch (FormatException) { invalid = true; }
    valid &= extra.Count == 2 && extra[0].Note == "a,b" && invalid;
    Console.WriteLine($"rows={rows.Count}; contract={(valid ? "PASS" : "FAIL")}");
    return valid ? 0 : 1;
}
Console.WriteLine($"rows={rows.Count}");
foreach (var row in rows) Console.WriteLine($"{row.Sku} | {row.Quantity} | {row.Note.Replace("\r", "\\r").Replace("\n", "\\n")}");

public record SupplyRow(string Sku, int Quantity, string Note);

public static class Importer
{
    public static List<SupplyRow> Parse(string csv)
    {
        var records = new List<List<string>>();
        var fields = new List<string>();
        var field = new StringBuilder();
        var quoted = false;
        var justClosedQuote = false;
        for (int i = 0; i < csv.Length; i++)
        {
            char c = csv[i];
            if (quoted)
            {
                if (c == '"' && i + 1 < csv.Length && csv[i + 1] == '"') { field.Append('"'); i++; }
                else if (c == '"') { quoted = false; justClosedQuote = true; }
                else field.Append(c);
                continue;
            }
            if (c == '"' && field.Length == 0 && !justClosedQuote) { quoted = true; continue; }
            if (c == ',')
            {
                fields.Add(field.ToString()); field.Clear(); justClosedQuote = false; continue;
            }
            if (c == '\r' || c == '\n')
            {
                if (c == '\r' && i + 1 < csv.Length && csv[i + 1] == '\n') i++;
                fields.Add(field.ToString()); field.Clear(); justClosedQuote = false;
                if (fields.Count > 1 || fields[0].Length > 0) records.Add(fields);
                fields = new List<string>();
                continue;
            }
            if (justClosedQuote || c == '"') throw new FormatException("Malformed CSV quoting");
            field.Append(c);
        }
        if (quoted) throw new FormatException("Unclosed quoted field");
        if (field.Length > 0 || fields.Count > 0)
        {
            fields.Add(field.ToString()); records.Add(fields);
        }
        if (records.Count == 0 || !records[0].SequenceEqual(new[] { "Sku", "Quantity", "Note" }))
            throw new FormatException("Unexpected CSV header");
        var result = new List<SupplyRow>();
        foreach (var r in records.Skip(1))
        {
            if (r.Count != 3 || !int.TryParse(r[1], out var quantity) || quantity < 0 || string.IsNullOrWhiteSpace(r[0]))
                throw new FormatException("Invalid supply row");
            result.Add(new SupplyRow(r[0], quantity, r[2]));
        }
        return result;
    }
}
