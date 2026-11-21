using System.Globalization;
namespace AuditSnapshotLab;
public record AidRecord(int Id, string Tenant, string Status, decimal Amount);
public sealed class ExportFilter { public string Tenant { get; set; } = "north"; public string Status { get; set; } = "Approved"; }
public static class ExportService
{
    public static IReadOnlyList<AidRecord> Prepare(IReadOnlyList<AidRecord> source, ExportFilter filter)
    {
        // Reporting adapter prepares the export while the UI may still edit its filter.
        IEnumerable<AidRecord> selected = source.Where(x => x.Tenant == filter.Tenant && x.Status == filter.Status);
        return new DeferredList(selected);
    }
}
public sealed class DeferredList(IEnumerable<AidRecord> source) : IReadOnlyList<AidRecord>
{
    public int Count => source.Count();
    public AidRecord this[int index] => source.ElementAt(index);
    public IEnumerator<AidRecord> GetEnumerator() => source.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
public static class Program
{
    private static readonly AidRecord[] Rows = [
        new(1,"north","Approved",10m), new(2,"north","Pending",20m),
        new(3,"south","Approved",30m), new(4,"north","Approved",40m)
    ];
    private static bool Check(string name, Func<bool> test) {
        try { bool ok = test(); Console.WriteLine($"{name}: {(ok ? "PASS" : "FAIL")}"); return ok; }
        catch (Exception ex) { Console.WriteLine($"{name}: FAIL ({ex.GetType().Name})"); return false; }
    }
    public static int Main(string[] args)
    {
        if (!args.Contains("--verify")) {
            var f = new ExportFilter();
            var result = ExportService.Prepare(Rows, f);
            f.Status = "Pending";
            Console.WriteLine("Export IDs: " + string.Join(",",result.Select(x=>x.Id)));
            return 0;
        }
        int failures = 0;
        void Assert(string name, Func<bool> check) { if (!Check(name,check)) failures++; }
        Assert("filter-edit-after-preparation", () => {
            var f = new ExportFilter();
            var result = ExportService.Prepare(Rows,f);
            f.Status = "Pending"; f.Tenant = "south";
            return result.Select(x=>x.Id).SequenceEqual(new[]{1,4});
        });
        Assert("source-change-after-preparation", () => {
            var data = Rows.ToList(); var f = new ExportFilter();
            var result = ExportService.Prepare(data,f);
            data.Add(new(5,"north","Approved",99m));
            return result.Select(x=>x.Id).SequenceEqual(new[]{1,4});
        });
        Assert("repeat-enumeration-stability", () => {
            var f = new ExportFilter(); var result = ExportService.Prepare(Rows,f);
            var first = result.Sum(x=>x.Amount);
            f.Status = "Pending";
            return first == 50m && result.Sum(x=>x.Amount) == first;
        });
        Assert("original-filter-precision", () => {
            var f = new ExportFilter {Tenant="south",Status="Approved"};
            return ExportService.Prepare(Rows,f).Select(x=>x.Id).SequenceEqual(new[]{3});
        });
        Assert("empty-result", () => {
            var f = new ExportFilter {Tenant="south",Status="Pending"};
            return ExportService.Prepare(Rows,f).Count==0;
        });
        return failures==0?0:1;
    }
}
