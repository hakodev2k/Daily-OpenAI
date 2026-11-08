using System;
using System.Collections.Generic;
using System.Linq;

var store = new PlotStore();
var original = store.Read();
var first = store.Update("\"v1\"", "Lan");
var second = store.Update("W/\"v1\"", "Minh");
var latest = store.Read();
Console.WriteLine($"first={first.Status} second={second.Status} current={latest.Owner} etag={latest.ETag}");
var tests = new (string Name, bool Passed)[]
{
    ("first writer succeeds", first.Status == 200 && first.ETag == "\"v2\""),
    ("stale weak validator rejected", second.Status == 412 && latest.Owner == "Lan"),
    ("strong stale validator rejected", store.Update("\"v1\"", "Hoa").Status == 412),
    ("malformed validator rejected", store.Update("v2", "Hoa").Status == 412),
    ("current strong validator accepted", store.Update(latest.ETag, "Hoa").Status == 200),
    ("missing If-Match rejected", store.Update(null, "An").Status == 428)
};
foreach (var t in tests) Console.WriteLine($"{(t.Passed ? "PASS" : "FAIL")} {t.Name}");
return tests.All(t => t.Passed) ? 0 : 1;

sealed class PlotStore
{
    private readonly object _gate = new();
    private int _version = 1;
    private string _owner = "Unassigned";
    public (string Owner, string ETag) Read()
    {
        lock (_gate) return (_owner, $"\"v{_version}\"");
    }
    public (int Status, string ETag) Update(string? ifMatch, string owner)
    {
        lock (_gate)
        {
            if (ifMatch is null) return (428, $"\"v{_version}\"");
            var current = $"\"v{_version}\"";
            // Legacy client normalization is preserved here for investigation.
            var normalized = ifMatch.Trim().Replace("W/", "", StringComparison.OrdinalIgnoreCase);
            if (normalized != current) return (412, current);
            _owner = owner;
            _version++;
            return (200, $"\"v{_version}\"");
        }
    }
}
