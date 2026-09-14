var cases = new[]
{
    new MoneyCase(1.005m, 1.01m),
    new MoneyCase(2.345m, 2.35m),
    new MoneyCase(2.355m, 2.36m),
    new MoneyCase(9.994m, 9.99m),
    new MoneyCase(9.996m, 10.00m)
};

var mismatchCount = cases.Count(test => RoundForPosting(test.Input) != test.Expected);
Console.WriteLine($"Mismatch count: {mismatchCount}");
return mismatchCount == 0 ? 0 : 1;

static decimal RoundForPosting(decimal amount)
    => Math.Round(amount, 2, MidpointRounding.AwayFromZero);

readonly record struct MoneyCase(decimal Input, decimal Expected);
