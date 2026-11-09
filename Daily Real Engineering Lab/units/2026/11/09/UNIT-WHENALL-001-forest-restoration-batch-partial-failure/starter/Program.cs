using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

record Survey(string Site, int SurvivingTrees);
record ImportResult(string Site, int? Trees, string? Error);

static class Program
{
    static async Task<Survey> ImportAsync(string site)
    {
        await Task.Yield();
        if (site == "ridge") throw new FormatException("invalid canopy reading");
        return new Survey(site, site == "valley" ? 18 : 12);
    }

    static async Task<ImportResult[]> ImportBatchAsync(string[] sites)
    {
        // This method was originally written when the upstream never rejected records.
        var tasks = sites.Select(ImportAsync).ToArray();
        try
        {
            var successful = await Task.WhenAll(tasks);
            return successful.Select(s => new ImportResult(s.Site, s.SurvivingTrees, null)).ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"batch rejected: {ex.GetType().Name}");
            return Array.Empty<ImportResult>();
        }
    }

    static async Task<int> Main(string[] args)
    {
        string[] sites = ["valley", "ridge", "plateau"];
        var results = await ImportBatchAsync(sites);
        foreach (var r in results) Console.WriteLine($"{r.Site}: {r.Trees?.ToString() ?? r.Error}");
        bool valid = results.Length == 3
            && results.Count(x => x.Error is null) == 2
            && results.SingleOrDefault(x => x.Site == "ridge")?.Error is not null
            && results.SingleOrDefault(x => x.Site == "valley")?.Trees == 18
            && results.SingleOrDefault(x => x.Site == "plateau")?.Trees == 12;
        Console.WriteLine(valid ? "PASS: all sites accounted for" : "FAIL: missing or incorrect site outcomes");
        return valid ? 0 : 1;
    }
}
