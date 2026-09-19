using System.Globalization;

var input = "19.95";
var cultures = new[] { "en-US", "de-DE" };
var results = new List<(string Culture, decimal Value)>();

foreach (var name in cultures)
{
    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(name);
    if (!decimal.TryParse(input, out var value))
    {
        Console.WriteLine($"{name}: PARSE_FAILED");
        return 4;
    }
    results.Add((name, value));
    Console.WriteLine($"{name}: {value.ToString(CultureInfo.InvariantCulture)}");
}

var consistent = results.All(x => x.Value == 19.95m);
if (args.Contains("reproduce"))
{
    if (!consistent) { Console.WriteLine("REPRODUCED"); return 2; }
    Console.WriteLine("NOT_REPRODUCED"); return 1;
}
if (args.Contains("verify"))
{
    if (consistent) { Console.WriteLine("VERIFY_PASS"); return 0; }
    Console.WriteLine("VERIFY_FAIL"); return 3;
}
return 0;
