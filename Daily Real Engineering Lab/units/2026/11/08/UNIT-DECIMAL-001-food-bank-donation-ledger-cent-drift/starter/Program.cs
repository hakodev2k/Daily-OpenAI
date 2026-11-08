using System.Globalization;

var amounts = args.Length == 0
    ? new[] { "0.005", "0.005", "0.005" }
    : args;

double total = 0;
foreach (var amount in amounts)
{
    var parsed = double.Parse(amount, CultureInfo.InvariantCulture);
    total += parsed;
}

var reported = Math.Round(total, 2, MidpointRounding.ToEven);
Console.WriteLine(reported.ToString("F2", CultureInfo.InvariantCulture));
