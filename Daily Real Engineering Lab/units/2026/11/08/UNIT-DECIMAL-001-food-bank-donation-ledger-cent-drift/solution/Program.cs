using System.Globalization;

var amounts = args.Length == 0
    ? new[] { "0.005", "0.005", "0.005" }
    : args;

decimal total = 0m;
foreach (var amount in amounts)
{
    var parsed = decimal.Parse(amount, NumberStyles.Number, CultureInfo.InvariantCulture);
    total += decimal.Round(parsed, 2, MidpointRounding.AwayFromZero);
}
Console.WriteLine(total.ToString("F2", CultureInfo.InvariantCulture));
