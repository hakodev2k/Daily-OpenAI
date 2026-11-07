using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

var received = new DateTimeOffset(2026, 11, 4, 10, 0, 0, TimeSpan.Zero);
var cases = new Probe[]
{
    new("date-future-429", 429, "Wed, 04 Nov 2026 10:00:08 GMT", received, 30, true, 8),
    new("date-future-503", 503, "Wed, 04 Nov 2026 10:00:12 GMT", received, 30, true, 12),
    new("date-past-503", 503, "Wed, 04 Nov 2026 09:59:55 GMT", received, 30, true, 0),
    new("date-over-budget", 429, "Wed, 04 Nov 2026 10:00:45 GMT", received, 20, false, 0),
    new("date-at-budget", 429, "Wed, 04 Nov 2026 10:00:20 GMT", received, 20, true, 20),
    new("delta-seconds", 429, "7", received, 30, true, 7),
    new("delta-over-budget", 503, "90", received, 20, false, 0),
    new("delta-zero", 429, "0", received, 30, true, 0),
    new("invalid-fallback", 429, "later", received, 30, true, 2),
    new("negative-fallback", 429, "-5", received, 30, true, 2),
    new("missing-fallback", 503, null, received, 30, true, 2),
    new("fallback-over-budget", 503, null, received, 1, false, 0),
    new("nonretryable-400", 400, "7", received, 30, false, 0)
};
var results = cases.Select(c => new { Case = c, Actual = RetryPlanner.Decide(
    c.StatusCode, c.RetryAfter, c.ReceivedAtUtc, TimeSpan.FromSeconds(c.BudgetSeconds)) }).ToArray();
var failures = new List<string>();
foreach (var row in results)
{
    bool passed = row.Actual.ShouldRetry == row.Case.ExpectedRetry
        && row.Actual.Delay == TimeSpan.FromSeconds(row.Case.ExpectedSeconds);
    Console.WriteLine($"{row.Case.Name}: status={row.Case.StatusCode}, header={row.Case.RetryAfter ?? "<none>"}, " +
        $"retry={row.Actual.ShouldRetry}, delay={row.Actual.Delay.TotalSeconds}s, " +
        $"expectedRetry={row.Case.ExpectedRetry}, expectedDelay={row.Case.ExpectedSeconds}s, " +
        $"result={(passed ? "PASS" : "FAIL")}");
    if (!passed) failures.Add(row.Case.Name);
}
bool allPassed = failures.Count == 0;
bool originalSymptom = failures.Count >= 3 &&
    failures.All(x => x.StartsWith("date-", StringComparison.Ordinal));
Console.WriteLine($"SUMMARY: passed={results.Length - failures.Count}/{results.Length}; " +
    $"originalSymptom={originalSymptom}; allPassed={allPassed}");
return (args.FirstOrDefault() ?? "inspect") switch
{
    "reproduce" => originalSymptom ? 0 : 2,
    "verify" => allPassed ? 0 : 3,
    "inspect" => 0,
    _ => 4
};

internal sealed record Probe(string Name, int StatusCode, string? RetryAfter,
    DateTimeOffset ReceivedAtUtc, int BudgetSeconds, bool ExpectedRetry, int ExpectedSeconds);
internal readonly record struct RetryDecision(bool ShouldRetry, TimeSpan Delay);
internal static class RetryPlanner
{
    public static RetryDecision Decide(int statusCode, string? retryAfter,
        DateTimeOffset receivedAtUtc, TimeSpan remainingBudget)
    {
        if (statusCode is not (429 or 503) || remainingBudget < TimeSpan.Zero)
            return new(false, TimeSpan.Zero);

        TimeSpan delay = TimeSpan.FromSeconds(2);
        var value = retryAfter?.Trim();
        if (!string.IsNullOrEmpty(value))
        {
            if (int.TryParse(value, NumberStyles.None,
                CultureInfo.InvariantCulture, out int seconds) && seconds >= 0)
            {
                delay = TimeSpan.FromSeconds(seconds);
            }
            else if (DateTimeOffset.TryParseExact(value, "r",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var retryAt))
            {
                delay = retryAt <= receivedAtUtc
                    ? TimeSpan.Zero
                    : retryAt - receivedAtUtc;
            }
        }

        if (delay > remainingBudget)
            return new(false, TimeSpan.Zero);
        return new(true, delay);
    }
}
