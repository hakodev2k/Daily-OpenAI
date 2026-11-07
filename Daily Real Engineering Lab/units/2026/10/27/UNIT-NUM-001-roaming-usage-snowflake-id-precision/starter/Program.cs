using RoamingUsage;

var cases = new[]
{
    new Case(
        "adjacent-snowflake-ids-and-real-retry",
        """[{"eventId":9007199254740992,"units":30},{"eventId":9007199254740993,"units":40},{"eventId":9007199254740992,"units":5}]""",
        new long[] { 9007199254740992L, 9007199254740993L }, 70),
    new Case(
        "ordinary-ids-and-duplicate",
        """[{"eventId":100,"units":2},{"eventId":100,"units":9},{"eventId":101,"units":7}]""",
        new long[] { 100L, 101L }, 9),
    new Case(
        "single-high-odd-id",
        """[{"eventId":9007199254740995,"units":12}]""",
        new long[] { 9007199254740995L }, 12)
};

var failed = false;
foreach (var test in cases)
{
    var actual = UsageImporter.Summarize(test.Json);
    var idsMatch = actual.AcceptedEventIds.SequenceEqual(test.ExpectedIds);
    var unitsMatch = actual.TotalUnits == test.ExpectedUnits;
    Console.WriteLine($"{test.Name}: ids=[{string.Join(",", actual.AcceptedEventIds)}], units={actual.TotalUnits}");
    if (!idsMatch || !unitsMatch)
    {
        Console.WriteLine($"CHECK_FAILED: {test.Name}; expected ids=[{string.Join(",", test.ExpectedIds)}], units={test.ExpectedUnits}");
        failed = true;
    }
    else
    {
        Console.WriteLine($"CHECK_OK: {test.Name}");
    }
}

return args.Contains("--check", StringComparer.OrdinalIgnoreCase) && failed ? 1 : 0;

internal sealed record Case(string Name, string Json, long[] ExpectedIds, int ExpectedUnits);
