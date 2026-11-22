using System.Globalization;
using System.Collections.Generic;

static string Normalize(string value) => value.ToUpper(CultureInfo.CurrentCulture);
static bool TryFind(string requested, Dictionary<string,string> records, out string? result)
    => records.TryGetValue(Normalize(requested), out result);
static Dictionary<string,string> CreateIndex(IEnumerable<(string Code,string Patient)> source)
{
    var index = new Dictionary<string,string>(StringComparer.Ordinal);
    foreach (var (code, patient) in source) index.Add(Normalize(code), patient);
    return index;
}
static int Check()
{
    var failures = 0;
    var cultures = new[] { "en-US", "tr-TR" };
    foreach (var culture in cultures)
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
        var index = CreateIndex(new[] { ("kit-i17", "sample-A"), ("KIT-I18", "sample-B") });
        var found = TryFind("KIT-I17", index, out var value);
        Console.WriteLine($"culture={culture}; found={found}; value={value ?? "<none>"}; keys={string.Join(",",index.Keys)}");
        if (!found || value != "sample-A") failures++;
    }
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    return failures;
}
var failures = Check();
if (args.Contains("--reproduce"))
{
    if (failures == 0) { Console.Error.WriteLine("Không reproduce được triệu chứng."); return 1; }
    Console.WriteLine($"Đã reproduce {failures} sai lệch hợp đồng."); return 0;
}
if (args.Contains("--verify"))
{
    if (failures != 0) { Console.Error.WriteLine($"FAIL: {failures} case; kiểm tra các culture."); return 1; }
    Console.WriteLine("PASS: key resolution ổn định qua culture."); return 0;
}
Console.WriteLine("Sử dụng --reproduce hoặc --verify.");
return 0;
