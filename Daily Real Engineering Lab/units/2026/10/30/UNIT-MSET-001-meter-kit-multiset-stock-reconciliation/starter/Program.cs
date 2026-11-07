using System;
using System.Collections.Generic;
using System.Linq;

// Một occurrence = một bộ kit vật lý. Mã SKU phân biệt chữ hoa/chữ thường.
var mode = args.Length > 0 ? args[0].ToLowerInvariant() : "demo";
var cases = new[]
{
    new Case("Thiếu một bộ", new[] {"KIT-A","KIT-A","KIT-B"}, new[] {"KIT-A","KIT-B"}, new[] {"KIT-A"}, Array.Empty<string>()),
    new Case("Thiếu hai bộ", new[] {"KIT-X","KIT-X","KIT-X"}, new[] {"KIT-X"}, new[] {"KIT-X","KIT-X"}, Array.Empty<string>()),
    new Case("Thừa hai bộ", new[] {"KIT-K"}, new[] {"KIT-K","KIT-Z","KIT-Z"}, Array.Empty<string>(), new[] {"KIT-Z","KIT-Z"}),
    new Case("Hoán vị hợp lệ", new[] {"KIT-A","KIT-B"}, new[] {"KIT-B","KIT-A"}, Array.Empty<string>(), Array.Empty<string>()),
    new Case("SKU phân biệt hoa thường", new[] {"kit-1"}, new[] {"KIT-1"}, new[] {"kit-1"}, new[] {"KIT-1"}),
    new Case("Chênh hai chiều", new[] {"KIT-P","KIT-P","KIT-Q"}, new[] {"KIT-P","KIT-R","KIT-R"}, new[] {"KIT-P","KIT-Q"}, new[] {"KIT-R","KIT-R"})
};
if (mode == "reproduce")
{
    var f = cases[0];
    var actual = Reconciler.Reconcile(f.Manifest, f.Scanned);
    Console.WriteLine($"Manifest: {Show(f.Manifest)}; Scanned: {Show(f.Scanned)}");
    Console.WriteLine($"Expected missing: {Show(f.Missing)}; Actual missing: {Show(actual.Missing)}");
    bool reproduced = actual.Missing.Count == 0 && f.Missing.SequenceEqual(new[] {"KIT-A"}, StringComparer.Ordinal);
    Console.WriteLine(reproduced ? "REPRODUCED: missing kit omitted." : "NOT REPRODUCED: original symptom absent.");
    return reproduced ? 0 : 1;
}
if (mode == "verify")
{
    int failed = 0;
    foreach (var f in cases)
    {
        var actual = Reconciler.Reconcile(f.Manifest, f.Scanned);
        bool ok = actual.Missing.SequenceEqual(f.Missing, StringComparer.Ordinal)
            && actual.Unexpected.SequenceEqual(f.Unexpected, StringComparer.Ordinal);
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {f.Name}: missing={Show(actual.Missing)}; unexpected={Show(actual.Unexpected)}");
        if (!ok) failed++;
    }
    Console.WriteLine($"Verification: {cases.Length - failed}/{cases.Length} passed.");
    return failed == 0 ? 0 : 1;
}
if (mode != "demo") { Console.Error.WriteLine("Use demo, reproduce or verify."); return 2; }
foreach (var f in cases)
{
    var r = Reconciler.Reconcile(f.Manifest, f.Scanned);
    Console.WriteLine($"{f.Name}: missing={Show(r.Missing)}; unexpected={Show(r.Unexpected)}");
}
return 0;

static string Show(IEnumerable<string> items) => "[" + string.Join(", ", items) + "]";
internal sealed record Case(string Name, IReadOnlyList<string> Manifest, IReadOnlyList<string> Scanned, IReadOnlyList<string> Missing, IReadOnlyList<string> Unexpected);
internal sealed record Result(IReadOnlyList<string> Missing, IReadOnlyList<string> Unexpected);
internal static class Reconciler
{
    public static Result Reconcile(IReadOnlyList<string> expected, IReadOnlyList<string> scanned)
    {
        return new Result(
            expected.Except(scanned, StringComparer.Ordinal).ToArray(),
            scanned.Except(expected, StringComparer.Ordinal).ToArray());
    }
}
