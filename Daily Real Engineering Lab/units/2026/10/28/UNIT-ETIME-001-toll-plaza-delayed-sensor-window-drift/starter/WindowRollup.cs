namespace GateWindowLab;

public sealed record GatePassage(
    string EventId,
    DateTimeOffset OccurredAt,
    DateTimeOffset ReceivedAt,
    int Delta);

public static class WindowRollup
{
    private static readonly long WindowTicks = TimeSpan.FromMinutes(5).Ticks;

    public static IReadOnlyDictionary<DateTimeOffset, int> Calculate(
        IEnumerable<GatePassage> passages)
    {
        var buckets = new SortedDictionary<DateTimeOffset, int>();
        foreach (var passage in passages.OrderBy(p => p.ReceivedAt))
        {
            DateTimeOffset timestamp = passage.ReceivedAt;
            long utcTicks = timestamp.UtcDateTime.Ticks;
            long startTicks = utcTicks - utcTicks % WindowTicks;
            var windowStartUtc = new DateTimeOffset(startTicks, TimeSpan.Zero);

            buckets.TryGetValue(windowStartUtc, out int current);
            buckets[windowStartUtc] = checked(current + passage.Delta);
        }
        return buckets;
    }
}
