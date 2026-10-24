using System.Globalization;
namespace BillingReconciliationLab;
internal sealed record Invoice(string Id, decimal[] Charges, decimal Settlement);
internal static class Program
{
    private static readonly Invoice[] Cases =
    [
        new("INV-201", [0.335m,0.335m,0.335m,0.335m], 1.34m),
        new("INV-202", [0.335m,0.335m,0.335m], 1.01m),
        new("INV-203", [0.335m,0.335m,0.335m,0.335m,-0.335m,-0.335m], 0.67m),
        new("INV-204", [-0.335m,-0.335m,-0.335m], -1.01m),
        new("INV-205", [1.20m,0.50m,-0.10m], 1.60m),
        new("INV-206", [0.335m], 0.34m),
        new("INV-207", [], 0m),
        new("INV-208", [0.12m,0.22m,0.005m,0.005m], 0.35m)
    ];
    private static int Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        return (args.Length == 0 ? "run" : args[0].ToLowerInvariant()) switch
        {
            "run" => Run(), "reproduce" => Reproduce(), "verify" => Verify(), _ => 2
        };
    }
    private static int Run()
    {
        Console.WriteLine("invoice | count | rawSum | local | partner | delta");
        foreach (var c in Cases)
        {
            var actual = InvoiceCalculator.Calculate(c.Charges);
            Console.WriteLine(string.Join(" | ", c.Id, c.Charges.Length, c.Charges.Sum(), actual, c.Settlement, actual-c.Settlement));
        }
        return 0;
    }
    private static int Reproduce()
    {
        string[] affected = ["INV-201","INV-202","INV-203","INV-204","INV-208"];
        var count = 0;
        foreach (var c in Cases)
        {
            var actual = InvoiceCalculator.Calculate(c.Charges);
            var mismatch = actual != c.Settlement;
            Console.WriteLine(c.Id + ": actual=" + actual + " settlement=" + c.Settlement + " mismatch=" + mismatch);
            if (mismatch) count++;
            if (affected.Contains(c.Id) != mismatch) return 1;
        }
        var ok = count == affected.Length;
        Console.WriteLine(ok ? "REPRODUCED" : "NOT REPRODUCED");
        return ok ? 0 : 1;
    }
    private static int Verify()
    {
        var failures = 0;
        foreach (var c in Cases)
        {
            var actual = InvoiceCalculator.Calculate(c.Charges);
            var ok = actual == c.Settlement;
            Console.WriteLine((ok ? "PASS " : "FAIL ") + c.Id + ": " + actual + " expected " + c.Settlement);
            if (!ok) failures++;
        }
        var split = InvoiceCalculator.Calculate([0.335m,0.335m,0.335m,0.335m]);
        var merged = InvoiceCalculator.Calculate([1.34m]);
        if (split != merged) { failures++; Console.WriteLine("FAIL grouping invariant"); }
        else Console.WriteLine("PASS grouping invariant");
        try { _ = InvoiceCalculator.Calculate(null!); failures++; Console.WriteLine("FAIL null contract"); }
        catch (ArgumentNullException) { Console.WriteLine("PASS null contract"); }
        Console.WriteLine(failures == 0 ? "VERIFIED" : "NOT VERIFIED: " + failures);
        return failures == 0 ? 0 : 1;
    }
}
