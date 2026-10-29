namespace MaintenanceScheduling;

internal static class ContractHarness
{
    private sealed record ContractCase(
        string Name, WindowRequest Request, DateTimeOffset? ExpectedUtc, bool ShouldReject);

    public static int Run(string[] args)
    {
        if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run --project Lab.csproj -- reproduce|verify");
            return 2;
        }

        static DateTime Local(int year, int month, int day, int hour, int minute) =>
            new(year, month, day, hour, minute, 0, DateTimeKind.Unspecified);
        static DateTimeOffset Utc(int year, int month, int day, int hour, int minute) =>
            new(year, month, day, hour, minute, 0, TimeSpan.Zero);

        var cases = new[]
        {
            new ContractCase("first-fold-with-offset",
                new("night-101", Local(2026, 11, 1, 1, 30), -240), Utc(2026, 11, 1, 5, 30), false),
            new ContractCase("second-fold-with-offset",
                new("night-102", Local(2026, 11, 1, 1, 30), -300), Utc(2026, 11, 1, 6, 30), false),
            new ContractCase("ambiguous-without-offset",
                new("night-103", Local(2026, 11, 1, 1, 30), null), null, true),
            new ContractCase("nonexistent-local-time",
                new("spring-201", Local(2026, 3, 8, 2, 30), -300), null, true),
            new ContractCase("ordinary-with-offset",
                new("day-301", Local(2026, 11, 1, 3, 30), -300), Utc(2026, 11, 1, 8, 30), false),
            new ContractCase("ordinary-wrong-offset",
                new("day-302", Local(2026, 11, 1, 3, 30), -240), null, true),
            new ContractCase("ordinary-without-offset",
                new("day-303", Local(2026, 10, 31, 14, 0), null), Utc(2026, 10, 31, 18, 0), false)
        };

        var failures = new HashSet<string>(StringComparer.Ordinal);
        foreach (var item in cases)
        {
            bool passed;
            string observation;
            try
            {
                var actual = WindowScheduler.Resolve(item.Request);
                passed = !item.ShouldReject && item.ExpectedUtc.HasValue && actual == item.ExpectedUtc.Value;
                observation = $"actual={actual:O}; expected={(item.ShouldReject ? "REJECT" : item.ExpectedUtc?.ToString("O"))}";
            }
            catch (ArgumentException ex)
            {
                passed = item.ShouldReject;
                observation = $"rejected={ex.GetType().Name}; expected={(item.ShouldReject ? "REJECT" : item.ExpectedUtc?.ToString("O"))}";
            }
            catch (Exception ex)
            {
                passed = false;
                observation = $"unexpected={ex.GetType().Name}: {ex.Message}";
            }

            if (!passed) failures.Add(item.Name);
            Console.WriteLine($"{(passed ? "PASS" : "FAIL")} {item.Name}: {observation}");
        }

        if (args[0] == "reproduce")
        {
            var expectedBaselineFailures = new HashSet<string>(StringComparer.Ordinal)
            {
                "first-fold-with-offset",
                "ambiguous-without-offset",
                "ordinary-wrong-offset"
            };
            bool baselineReproduced = failures.SetEquals(expectedBaselineFailures);
            Console.WriteLine(baselineReproduced
                ? "REPRODUCED: exactly three expected starter contract failures."
                : "BASELINE CHANGED: expected failure set was not reproduced.");
            return baselineReproduced ? 0 : 1;
        }

        Console.WriteLine(failures.Count == 0
            ? "VERIFIED: all seven contract cases passed."
            : $"NOT VERIFIED: {failures.Count} contract case(s) failed.");
        return failures.Count == 0 ? 0 : 1;
    }
}
