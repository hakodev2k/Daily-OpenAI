using System.Globalization;
namespace KioskBackoff;
public static class RetryPolicy
{
    public static TimeSpan ComputeDelay(string? header, DateTimeOffset now, TimeSpan fallback)
    {
        if (int.TryParse(header, NumberStyles.Integer, CultureInfo.InvariantCulture, out int seconds))
            return TimeSpan.FromSeconds(Math.Max(0, seconds));
        return fallback;
    }
}
public static class Program
{
    public static int Main(string[] args)
    {
        var now = new DateTimeOffset(2026, 11, 22, 3, 0, 0, TimeSpan.Zero);
        var cases = new (string Name, string Header, int Seconds)[] {
            ("delta", "15", 15),
            ("future-date", "Sun, 22 Nov 2026 03:02:00 GMT", 120),
            ("past-date", "Sun, 22 Nov 2026 02:59:00 GMT", 0),
            ("invalid", "invalid-header", 5)
        };
        int failures = 0;
        foreach (var item in cases)
        {
            var actual = RetryPolicy.ComputeDelay(item.Header, now, TimeSpan.FromSeconds(5));
            bool pass = actual == TimeSpan.FromSeconds(item.Seconds);
            Console.WriteLine($"{item.Name}: expected={item.Seconds} actual={actual.TotalSeconds} {(pass ? "PASS" : "FAIL")}");
            if (!pass) failures++;
        }
        Console.WriteLine($"CONTRACT_FAILURES={failures}");
        if (args.Contains("--verify")) return failures == 0 ? 0 : 1;
        if (args.Contains("--reproduce")) return failures > 0 ? 0 : 1;
        return 0;
    }
}
