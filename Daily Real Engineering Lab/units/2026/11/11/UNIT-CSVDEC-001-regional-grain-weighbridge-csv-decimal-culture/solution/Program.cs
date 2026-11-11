using System.Globalization;

var cultureName = args.Length > 0 ? args[0] : "en-US";
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
var csv = "WB-492,1250.75";
var parts = csv.Split(',');
const NumberStyles allowed = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
if (parts.Length != 2 || !decimal.TryParse(parts[1], allowed, CultureInfo.InvariantCulture, out var kilograms))
{
    Console.Error.WriteLine($"IMPORT_FAILED culture={CultureInfo.CurrentCulture.Name} raw={csv}");
    return 2;
}
Console.WriteLine($"IMPORTED culture={CultureInfo.CurrentCulture.Name} ticket={parts[0]} kilograms={kilograms.ToString(CultureInfo.InvariantCulture)}");
return kilograms == 1250.75m ? 0 : 3;
