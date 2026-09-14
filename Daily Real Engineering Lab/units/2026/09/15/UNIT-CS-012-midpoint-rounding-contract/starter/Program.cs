var mode = args.FirstOrDefault() ?? "--reproduce";

var cases = new[]
{
    new MoneyCase(1.005m, 1.01m),
    new MoneyCase(2.345m, 2.35m),
    new MoneyCase(2.355m, 2.36m),
    new MoneyCase(9.994m, 9.99m),
    new MoneyCase(9.996m, 10.00m)
};

var results = cases
    .Select(test => new MoneyResult(test.Input, RoundForPosting(test.Input), test.Expected))
    .ToArray();

foreach (var result in results)
{
    var status = result.Actual == result.Expected ? "PASS" : "FAIL";
    Console.WriteLine($"{status} input={result.Input:0.000} actual={result.Actual:0.00} expected={result.Expected:0.00}");
}

var mismatchCount = results.Count(result => result.Actual != result.Expected);
Console.WriteLine($"Mismatch count: {mismatchCount}");

if (mode == "--reproduce")
{
    var symptomObserved = mismatchCount > 0 && mismatchCount < results.Length;
    Console.WriteLine(symptomObserved ? "REPRODUCED" : "REPRODUCE FAILED");
    return symptomObserved ? 0 : 1;
}

if (mode == "--verify")
{
    var verified = mismatchCount == 0;
    Console.WriteLine(verified ? "VERIFIED" : "VERIFY FAILED");
    return verified ? 0 : 1;
}

return 2;

static decimal RoundForPosting(decimal amount)
{
    // Investigation note:
    // The accounting system has an explicit rounding contract.
    // Does this overload make that contract explicit?
    return Math.Round(amount, 2);
}

readonly record struct MoneyCase(decimal Input, decimal Expected);
readonly record struct MoneyResult(decimal Input, decimal Actual, decimal Expected);
