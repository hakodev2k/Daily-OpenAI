using System.Globalization;
using System.Collections.Generic;

static Dictionary<string,string> CreateIndex(IEnumerable<(string Code,string Patient)> source)
{
    var index = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
    foreach (var (code, patient) in source) index.Add(code, patient);
    return index;
}
static int Check()
{
    int failures = 0;
    foreach (var culture in new[] { "en-US", "tr-TR" })
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
        var index = CreateIndex(new[] { ("kit-i17", "sample-A"), ("KIT-I18", "sample-B") });
        var found = index.TryGetValue("KIT-I17", out var value);
        Console.WriteLine($"culture={culture}; found={found}; value={value ?? "<none>"}");
        if (!found || value != "sample-A") failures++;
    }
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    return failures;
}
var failures = Check();
if (args.Contains("--reproduce")) return failures == 0 ? 1 : 0;
if (args.Contains("--verify")) { Console.WriteLine(failures == 0 ? "PASS" : "FAIL"); return failures == 0 ? 0 : 1; }
return 0;
