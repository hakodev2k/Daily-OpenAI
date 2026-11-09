using HarborInventory;
var labels = new[] { "BX.04", "BXZ04", "P+7", "P7", "[A]", "A", "Q(1)", "Q1", "CARGO-01", "cargo-01" };
var tests = new (string Scan, string[] Expected)[]
{
    ("BX.04", new[] { "BX.04" }), ("P+7", new[] { "P+7" }),
    ("[A]", new[] { "[A]" }), ("Q(1)", new[] { "Q(1)" }),
    ("CARGO-01", new[] { "CARGO-01" }),
    ("cargo-01", new[] { "cargo-01" }),
    ("NOT-FOUND", Array.Empty<string>())
};
int failures = 0;
foreach (var test in tests)
{
    try {
        var actual = ContainerLookup.Find(labels, test.Scan);
        bool pass = actual.SequenceEqual(test.Expected);
        Console.WriteLine($"{test.Scan}: {(pass ? "PASS" : "FAIL")} [{string.Join(",", actual)}]");
        if (!pass) failures++;
    } catch (Exception ex) { Console.WriteLine($"{test.Scan}: FAIL {ex.GetType().Name}"); failures++; }
}
Console.WriteLine($"Failures: {failures}");
return failures == 0 ? 0 : 1;
