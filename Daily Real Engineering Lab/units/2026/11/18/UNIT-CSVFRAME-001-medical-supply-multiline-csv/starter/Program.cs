using System.Text;
using System.Linq;

var csv = "Sku,Quantity,Note\r\nGLOVE-01,12,\"Kho tầng 2\r\nkiểm kê lại\"\r\nMASK-02,8,\"Ưu tiên \"\"khoa cấp cứu\"\"\"\r\n";
var rows = Importer.Parse(csv);
if (args.Contains("--check"))
{
    var valid = rows.Count == 2
        && rows[0].Sku == "GLOVE-01"
        && rows[0].Quantity == 12
        && rows[0].Note == "Kho tầng 2\r\nkiểm kê lại"
        && rows[1].Sku == "MASK-02"
        && rows[1].Quantity == 8
        && rows[1].Note == "Ưu tiên \"khoa cấp cứu\"";
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
        var result = new List<SupplyRow>();
        foreach (var line in csv.Split('\n').Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var columns = line.TrimEnd('\r').Split(',');
            if (columns.Length != 3 || !int.TryParse(columns[1], out var quantity))
                continue;
            result.Add(new SupplyRow(columns[0], quantity, columns[2].Trim('"')));
        }
        return result;
    }
}
