using System.Globalization;

static long AggregateWh(IEnumerable<int> inverterReadings)
{
    long total = 0;
    foreach (var wh in inverterReadings)
    {
        if (wh < 0) throw new ArgumentOutOfRangeException(nameof(inverterReadings));
        total = checked(total + wh);
    }
    return total;
}
var scenarios = new Dictionary<string, int[]>
{
    ["small"] = [120_000, 240_000, 360_000],
    ["peak"] = [1_500_000_000, 1_200_000_000, 600_000_000],
    ["boundary"] = [int.MaxValue, 1]
};
foreach (var (name, values) in scenarios)
{
    long actual = AggregateWh(values);
    long expected = values.Sum(v => (long)v);
    Console.WriteLine($"INPUT {name} count={values.Length}");
    Console.WriteLine($"ACTUAL {name} {actual.ToString(CultureInfo.InvariantCulture)}");
    Console.WriteLine($"EXPECTED {name} {expected.ToString(CultureInfo.InvariantCulture)}");
}
