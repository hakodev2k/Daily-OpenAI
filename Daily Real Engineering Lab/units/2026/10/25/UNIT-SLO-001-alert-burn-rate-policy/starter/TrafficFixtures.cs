namespace SloAlertLab;

internal readonly record struct MinuteBucket(int Requests, int Errors);

internal static class TrafficFixtures
{
    private static MinuteBucket[] Generate(Func<int, MinuteBucket> factory) =>
        Enumerable.Range(0, 30).Select(factory).ToArray();

    public static MinuteBucket[] LowVolumeOutage() =>
        Generate(_ => new MinuteBucket(100, 12));

    public static MinuteBucket[] HighVolumeWithinBudget() =>
        Generate(_ => new MinuteBucket(30_000, 25));

    public static MinuteBucket[] BriefSpike() =>
        Generate(i => new MinuteBucket(100, i == 29 ? 15 : 0));

    public static MinuteBucket[] ModerateDrift() =>
        Generate(_ => new MinuteBucket(1_000, 2));

    public static MinuteBucket[] ZeroTraffic() =>
        Generate(_ => new MinuteBucket(0, 0));

    public static MinuteBucket[] RecoveredIncident() =>
        Generate(i => new MinuteBucket(100, i < 15 ? 12 : 0));
}
