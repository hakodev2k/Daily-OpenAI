namespace SloAlertLab;

internal static class Program
{
    private static readonly (string Name, Func<MinuteBucket[]> Data, bool ExpectedPage)[] Scenarios =
    [
        ("low-volume sustained outage", TrafficFixtures.LowVolumeOutage, true),
        ("high-volume within SLO", TrafficFixtures.HighVolumeWithinBudget, false),
        ("single-minute spike", TrafficFixtures.BriefSpike, false),
        ("moderate drift", TrafficFixtures.ModerateDrift, false),
        ("zero traffic", TrafficFixtures.ZeroTraffic, false),
        ("incident recovered", TrafficFixtures.RecoveredIncident, false)
    ];

    private static int Main(string[] args)
    {
        var command = args.Length == 0 ? "observe" : args[0].ToLowerInvariant();
        return command switch
        {
            "observe" => Observe(),
            "reproduce" => Reproduce(),
            "verify" => Verify(),
            _ => Usage()
        };
    }

    private static int Usage()
    {
        Console.Error.WriteLine("Usage: dotnet run -- [observe|reproduce|verify]");
        return 2;
    }

    private static int Observe()
    {
        foreach (var (name, data, expected) in Scenarios)
        {
            var buckets = data();
            var requests = buckets.Sum(x => (long)x.Requests);
            var errors = buckets.Sum(x => (long)x.Errors);
            var rate = requests == 0 ? "N/A" : $"{(double)errors / requests:P3}";
            Console.WriteLine($"{name,-30} requests={requests,8} errors={errors,5} rate={rate,9} page={AlertPolicy.ShouldPage(buckets)} expected={expected}");
        }
        return 0;
    }

    private static int Reproduce()
    {
        var missed = !AlertPolicy.ShouldPage(TrafficFixtures.LowVolumeOutage());
        var falsePage = AlertPolicy.ShouldPage(TrafficFixtures.HighVolumeWithinBudget());
        Console.WriteLine($"Sustained outage missed: {missed}");
        Console.WriteLine($"Healthy peak paged: {falsePage}");
        Console.WriteLine(missed && falsePage ? "REPRODUCED" : "NOT REPRODUCED");
        return missed && falsePage ? 0 : 1;
    }

    private static int Verify()
    {
        var failed = 0;
        foreach (var (name, data, expected) in Scenarios)
        {
            var actual = AlertPolicy.ShouldPage(data());
            if (actual != expected)
            {
                Console.Error.WriteLine($"FAIL: {name}; expected page={expected}, actual={actual}");
                failed++;
            }
            else Console.WriteLine($"PASS: {name}");
        }
        Console.WriteLine(failed == 0 ? "PASS: all behavioral cases" : $"FAIL: {failed} cases");
        return failed == 0 ? 0 : 1;
    }
}
