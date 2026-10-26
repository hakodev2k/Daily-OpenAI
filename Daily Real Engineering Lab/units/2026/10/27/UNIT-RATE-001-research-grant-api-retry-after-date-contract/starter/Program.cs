using System.Globalization;
using System.Net;
using System.Net.Http;

internal static class Program
{
    private static readonly DateTimeOffset FixedUtc = new(2026, 10, 27, 0, 0, 0, TimeSpan.Zero);
    private sealed record Fixture(string Name, string? Header, int ExpectedSeconds);

    public static int Main()
    {
        var cases = new[]
        {
            new Fixture("delta-seconds", "7", 7),
            new Fixture("http-date-future", FixedUtc.AddSeconds(11).ToString("r", CultureInfo.InvariantCulture), 11),
            new Fixture("http-date-expired", FixedUtc.AddSeconds(-9).ToString("r", CultureInfo.InvariantCulture), 0),
            new Fixture("http-date-over-budget", FixedUtc.AddSeconds(90).ToString("r", CultureInfo.InvariantCulture), 30),
            new Fixture("delta-over-budget", "120", 30),
            new Fixture("missing-header", null, 2),
            new Fixture("malformed-header", "not-a-retry-window", 2),
            new Fixture("zero-delta", "0", 0)
        };
        int failed = 0;
        foreach (var test in cases)
        {
            using var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
            if (test.Header is not null &&
                !response.Headers.TryAddWithoutValidation("Retry-After", test.Header))
            {
                Console.Error.WriteLine($"Invalid fixture setup: {test.Name}");
                return 13;
            }
            var actual = RetryPlanner.GetDelay(response, FixedUtc,
                TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30));
            bool pass = actual == TimeSpan.FromSeconds(test.ExpectedSeconds);
            Console.WriteLine($"{(pass ? "PASS" : "FAIL")} {test.Name,-23} expected={test.ExpectedSeconds,2}s actual={actual.TotalSeconds,2}s");
            if (!pass) failed++;
        }
        Console.WriteLine($"Summary: {cases.Length - failed}/{cases.Length} fixtures passed.");
        return failed == 0 ? 0 : 12;
    }
}
