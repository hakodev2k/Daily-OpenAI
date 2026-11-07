using ClockLab;

internal static class Program
{
    private static readonly DateTimeOffset Origin =
        new(2026, 10, 24, 8, 0, 0, TimeSpan.Zero);

    private sealed record CaseResult(string Name, bool Expected, bool Actual)
    {
        public bool Passed => Expected == Actual;
    }

    private static CaseResult Check(
        string name, TimeSpan elapsed, TimeSpan wallAdjustment, bool expected)
    {
        var clock = new ManualTimeProvider(Origin);
        var watchdog = new LeaseWatchdog(clock, TimeSpan.FromSeconds(10));
        watchdog.Start();
        clock.Advance(elapsed);
        clock.AdjustWallClock(wallAdjustment);
        return new CaseResult(name, expected, watchdog.IsExpired());
    }

    private static CaseResult CheckRearm()
    {
        var clock = new ManualTimeProvider(Origin);
        var watchdog = new LeaseWatchdog(clock, TimeSpan.FromSeconds(10));
        watchdog.Start();
        clock.Advance(TimeSpan.FromSeconds(12));
        watchdog.Start();
        clock.Advance(TimeSpan.FromSeconds(3));
        return new CaseResult("rearm-after-expiration", false, watchdog.IsExpired());
    }

    private static List<CaseResult> RunCases() =>
    [
        Check("stable-before-budget", TimeSpan.FromSeconds(9), TimeSpan.Zero, false),
        Check("stable-at-budget", TimeSpan.FromSeconds(10), TimeSpan.Zero, true),
        Check("clock-forward", TimeSpan.FromSeconds(4), TimeSpan.FromHours(2), false),
        Check("clock-backward", TimeSpan.FromSeconds(12), TimeSpan.FromHours(-2), true),
        CheckRearm()
    ];

    private static void Print(IEnumerable<CaseResult> cases)
    {
        foreach (var item in cases)
            Console.WriteLine(
                $"{item.Name,-24} expected={item.Expected,-5} actual={item.Actual,-5} " +
                $"result={(item.Passed ? "PASS" : "FAIL")}");
    }

    private static int Main(string[] args)
    {
        var mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
        var cases = RunCases();
        Print(cases);

        if (mode == "demo")
        {
            Console.WriteLine("Demo: xem kết quả, không dùng exit code để đánh giá fix.");
            return 0;
        }
        if (mode == "reproduce")
        {
            var controlsOk = cases
                .Where(x => x.Name != "clock-forward" && x.Name != "clock-backward")
                .All(x => x.Passed);
            var failures = cases
                .Where(x => x.Name is "clock-forward" or "clock-backward")
                .Count(x => !x.Passed);
            var reproduced = controlsOk && failures == 2;
            Console.WriteLine(reproduced
                ? "REPRODUCED: cả hai triệu chứng đã được chứng minh."
                : "NOT REPRODUCED: kiểm tra control cases.");
            return reproduced ? 0 : 1;
        }
        if (mode == "verify")
        {
            var success = cases.All(x => x.Passed);
            Console.WriteLine(success
                ? "VERIFIED: tất cả behavior assertions PASS."
                : "FAILED: ít nhất một behavior assertion chưa đạt.");
            return success ? 0 : 1;
        }
        Console.Error.WriteLine("Mode hợp lệ: demo | reproduce | verify");
        return 2;
    }
}
