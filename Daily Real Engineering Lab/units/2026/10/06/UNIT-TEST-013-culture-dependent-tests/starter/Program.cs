using System.Globalization;
using CultureLab;

if (args.Length != 1) throw new ArgumentException("Expected culture name.");
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(args[0]);

var input = new DateOnly(2026, 10, 5);
var actual = PartnerDateFormatter.Format(input);
const string expected = "2026-10-05";

Console.WriteLine($"CULTURE={CultureInfo.CurrentCulture.Name}");
Console.WriteLine($"INPUT={input:yyyy-MM-dd}");
Console.WriteLine($"ACTUAL={actual}");
Console.WriteLine($"EXPECTED={expected}");

return actual == expected ? 0 : 1;
