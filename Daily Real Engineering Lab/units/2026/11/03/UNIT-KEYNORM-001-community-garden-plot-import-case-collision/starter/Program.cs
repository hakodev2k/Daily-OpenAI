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
        result[plot.Code] = plot;
    return result;
}
var stored = Import(rows);
Console.WriteLine($"InputRows={rows.Length}");
Console.WriteLine($"StoredPlots={stored.Count}");
Console.WriteLine($"Owners={string.Join(",", stored.Values.Select(x => x.Owner).OrderBy(x => x))}");
