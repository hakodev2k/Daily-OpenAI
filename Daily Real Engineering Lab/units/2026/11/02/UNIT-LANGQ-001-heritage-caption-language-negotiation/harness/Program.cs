using CaptionLab;

record Scenario(string Name, string? Header, string? Expected);

static class Program
{
    static readonly Scenario[] Cases =
    {
        new("missing-header", null, "en"),
        new("empty-header", "", "en"),
        new("single-vi", "vi", "vi"),
        new("weighted-preference", "en;q=0.2, vi;q=0.9", "vi"),
        new("explicit-exclusion", "vi;q=0, en;q=1", "en"),
        new("regional-language", "fr-CA, en;q=0.8", "fr"),
        new("unsupported-first", "de, fr;q=0.7", "fr"),
        new("unsupported-only", "de, es;q=0.2", null),
        new("wildcard", "de, *;q=0.5", "en"),
        new("wildcard-with-exclusion", "en;q=0, *;q=0.5", "vi"),
        new("all-excluded", "en;q=0, vi;q=0, fr;q=0", null),
        new("tie-respects-header-order", "fr;q=0.8, vi;q=0.8", "fr"),
        new("mixed-case-region", "FR-ca;q=0.7", "fr"),
        new("lower-priority-region", "vi-VN;q=0.7, fr;q=0.9", "fr"),
        new("zero-wildcard", "*;q=0", null),
        new("invalid-weight-skipped", "vi;q=2, en;q=0.5", "en")
    };

    static string Label(string? value) => value ?? "<406>";

    static int Main(string[] args)
    {
        string mode = args.Length == 1 ? args[0].ToLowerInvariant() : "inspect";
        if (mode != "inspect" && mode != "reproduce" && mode != "verify")
        {
            Console.Error.WriteLine("Usage: inspect | reproduce | verify");
            return 2;
        }

        int failures = 0;
        var mismatches = new HashSet<string>(StringComparer.Ordinal);
        foreach (var item in Cases)
        {
            string? actual = CaptionLocale.Select(item.Header);
            bool ok = string.Equals(actual, item.Expected, StringComparison.Ordinal);
            Console.WriteLine($"{item.Name,-27} header={item.Header ?? "<none>",-35} actual={Label(actual),-6} expected={Label(item.Expected),-6} {(ok ? "PASS" : "FAIL")}");
            if (!ok) { failures++; mismatches.Add(item.Name); }
        }

        if (mode == "inspect") return 0;
        if (mode == "verify")
        {
            Console.WriteLine(failures == 0 ? "VERIFIED: all behavior contracts pass." : $"NOT VERIFIED: {failures} contract(s) failed.");
            return failures == 0 ? 0 : 1;
        }

        string[] required = { "weighted-preference", "explicit-exclusion", "regional-language", "unsupported-first" };
        bool reproduced = required.All(mismatches.Contains) && !mismatches.Contains("missing-header") && !mismatches.Contains("single-vi");
        Console.WriteLine(reproduced ? "REPRODUCED: known baseline symptoms confirmed." : "NOT REPRODUCED: baseline differs from expected failure profile.");
        return reproduced ? 0 : 1;
    }
}
