record Plot(string Code, string Owner);
var rows = new[] {
    new Plot("A-17", "Mai"),
    new Plot("B-04", "Nam"),
    new Plot("a-17", "Linh"),
    new Plot("C-08", "Hoa")
};
static Dictionary<string, Plot> Import(IEnumerable<Plot> source)
{
    var result = new Dictionary<string, Plot>(StringComparer.OrdinalIgnoreCase);
    foreach (var plot in source)
        if (!result.TryAdd(plot.Code, plot))
            throw new InvalidOperationException($"Duplicate plot identity: {plot.Code}");
    return result;
}
var committed = false;
var rejected = false;
try { _ = Import(rows); committed = true; }
catch (InvalidOperationException) { rejected = true; }
var unique = Import(new[] { rows[0], rows[1], rows[3] });
Console.WriteLine($"DuplicateRejected={rejected}");
Console.WriteLine($"OriginalBatchCommitted={committed}");
Console.WriteLine($"UniqueBatchStored={unique.Count}");
