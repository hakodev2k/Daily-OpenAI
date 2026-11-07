using System.Text;

var mode = args.Length > 0 ? args[0] : "demo";
const string csv = "BatchId,Species,Count,Note\r\nA-101,Pine,12,OK\r\nA-102,Oak,7,\"North, ridge\r\nSupervisor said \"\"hold\"\" until rain\"\r\nA-103,Birch,5,Ready\r\n";
const string simple = "BatchId,Species,Count,Note\r\nB-201,Pine,4,Ready\r\nB-202,Oak,6,OK\r\n";
const string invalid = "BatchId,Species,Count,Note\r\nX-001,Oak,2,\"unfinished\r\n";
try
{
    if (mode == "demo")
    {
        foreach (var row in CsvImporter.Parse(csv))
            Console.WriteLine($"{row.BatchId} | {row.Species} | {row.Count} | {row.Note.Replace("\r", "\\r").Replace("\n", "\\n")}");
        return;
    }
    if (mode != "test") throw new ArgumentException("Expected demo or test");
    var errors = new List<string>();
    Check("simple rows", () => CsvImporter.Parse(simple).Count == 2, errors);
    Check("multiline records", () =>
    {
        var rows = CsvImporter.Parse(csv);
        return rows.Count == 3 && rows[0].BatchId == "A-101" && rows[1].BatchId == "A-102"
            && rows[1].Note == "North, ridge\r\nSupervisor said \"hold\" until rain"
            && rows[2].BatchId == "A-103" && rows[2].Count == 5;
    }, errors);
    Check("unclosed quote rejected", () =>
    {
        try { CsvImporter.Parse(invalid); return false; }
        catch (FormatException) { return true; }
    }, errors);
    if (errors.Count > 0)
    {
        foreach (var error in errors) Console.Error.WriteLine("FAIL " + error);
        Environment.ExitCode = 1;
    }
    else Console.WriteLine("PASS all 3 contracts");
}
catch (Exception e) { Console.Error.WriteLine(e); Environment.ExitCode = 2; }

static void Check(string name, Func<bool> check, List<string> errors)
{
    try { if (!check()) errors.Add(name); }
    catch (Exception e) { errors.Add(name + ": " + e.GetType().Name + " " + e.Message); }
}

public record NurseryBatch(string BatchId, string Species, int Count, string Note);

public static class CsvImporter
{
    public static List<NurseryBatch> Parse(string csv)
    {
        var lines = csv.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var output = new List<NurseryBatch>();
        foreach (var line in lines.Skip(1))
        {
            var columns = line.TrimEnd('\r').Split(',');
            if (columns.Length != 4 || !int.TryParse(columns[2], out var count))
                throw new FormatException("Invalid nursery CSV record");
            output.Add(new NurseryBatch(columns[0], columns[1], count, columns[3]));
        }
        return output;
    }
}
