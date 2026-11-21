using System.Globalization;

var culture = args.Length > 0 ? args[0] : "en-US";
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
Console.WriteLine($"culture={CultureInfo.CurrentCulture.Name}");
string[] lines = ["S-41|12.75", "S-42|-3.50"];
foreach (var line in lines)
{
    var fields = line.Split('|');
    if (fields.Length != 2)
    {
        Console.WriteLine($"REJECT {line}");
        continue;
    }

    if (decimal.TryParse(fields[1], out var temperature))
        Console.WriteLine($"ACCEPT {fields[0]} {temperature.ToString(CultureInfo.InvariantCulture)}");
    else
        Console.WriteLine($"REJECT {line}");
}
