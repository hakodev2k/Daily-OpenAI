using System.Net;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;

var cases = new (string Name, string IssuedPass)[]
{
    ("ordinary", "LIFT-2048"),
    ("zone", "SKI+NORTH/27=="),
    ("category", "CABIN&ZONE=4"),
    ("badge", "SNOW#VIP"),
    ("literal-percent", "PCT%2B"),
    ("international", "VÉ+29")
};

var mode = args.Length == 0 ? "--run" : args[0];
if (mode == "--reproduce")
{
    var selected = cases[1];
    var uri = PartnerClient.BuildValidationUri(selected.IssuedPass);
    var received = PartnerSimulator.ReadPass(uri);
    Console.WriteLine($"ISSUED={selected.IssuedPass}");
    Console.WriteLine($"REQUEST={uri.AbsoluteUri}");
    Console.WriteLine($"PARTNER_RECEIVED={received ?? "<missing>"}");
    if (string.Equals(selected.IssuedPass, received, StringComparison.Ordinal))
    {
        Console.WriteLine("REPRO_MISMATCH: expected starter symptom was not observed");
        return 1;
    }
    Console.WriteLine("REPRO_FAIL: partner received a different pass identity");
    return 2;
}

if (mode == "--verify")
{
    var failures = 0;
    foreach (var item in cases)
    {
        var uri = PartnerClient.BuildValidationUri(item.IssuedPass);
        var received = PartnerSimulator.ReadPass(uri);
        if (string.Equals(item.IssuedPass, received, StringComparison.Ordinal))
        {
            Console.WriteLine($"PASS {item.Name}");
        }
        else
        {
            failures++;
            Console.WriteLine($"VERIFY_FAIL {item.Name}: expected='{item.IssuedPass}', actual='{received ?? "<missing>"}'");
        }
    }
    if (failures != 0)
    {
        Console.WriteLine($"VERIFY_FAIL: {failures} of {cases.Length} contract cases");
        return 3;
    }
    Console.WriteLine($"VERIFY_PASS: {cases.Length} of {cases.Length} contract cases");
    return 0;
}

if (mode == "--run")
{
    Console.WriteLine("Local partner contract fixture; no network calls.");
    foreach (var item in cases)
    {
        var uri = PartnerClient.BuildValidationUri(item.IssuedPass);
        var received = PartnerSimulator.ReadPass(uri);
        var status = string.Equals(item.IssuedPass, received, StringComparison.Ordinal)
            ? "ACCEPTED" : "REJECTED";
        Console.WriteLine($"{item.Name}: {status}");
    }
    return 0;
}

Console.Error.WriteLine("Usage: --run | --reproduce | --verify");
return 64;

internal static class PartnerSimulator
{
    // Models the partner's standard query-form decoding contract.
    public static string? ReadPass(Uri requestUri)
    {
        var pair = requestUri.Query.TrimStart('?')
            .Split('&', StringSplitOptions.None)
            .FirstOrDefault(part => part.StartsWith("pass=", StringComparison.Ordinal));
        return pair is null ? null : WebUtility.UrlDecode(pair["pass=".Length..]);
    }
}
