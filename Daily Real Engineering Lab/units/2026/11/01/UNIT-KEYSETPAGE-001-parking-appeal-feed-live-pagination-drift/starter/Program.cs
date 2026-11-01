public sealed record Appeal(long Id, DateTimeOffset CreatedAt, string Status);
public sealed record PageCursor(int Offset, DateTimeOffset? LastCreatedAt, long? LastId);
public sealed record PageResult(IReadOnlyList<Appeal> Items, PageCursor? Next);

public static class AppealFeed
{
    public static PageResult Fetch(IReadOnlyList<Appeal> records, int pageSize, PageCursor? cursor = null)
    {
        if (pageSize is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        // The feed is sorted newest-first; every HTTP request reads a fresh snapshot.
        var ordered = records.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id);
        var offset = cursor?.Offset ?? 0;
        var window = ordered.Skip(offset).Take(pageSize + 1).ToArray();
        var items = window.Take(pageSize).ToArray();
        var next = window.Length > pageSize
            ? new PageCursor(offset + items.Length, null, null)
            : null;

        return new PageResult(items, next);
    }
}

public static class DemoData
{
    public static readonly DateTimeOffset BaseTime =
        new(2026, 11, 1, 12, 0, 0, TimeSpan.Zero);

    public static List<Appeal> Initial() =>
        Enumerable.Range(101, 6)
            .Select(id => new Appeal(id, BaseTime.AddMinutes(id - 100), "PENDING"))
            .ToList();

    public static Appeal NewAppeal() => new(107, BaseTime.AddMinutes(7), "PENDING");
}

public static class Program
{
    public static int Main(string[] args)
    {
        var feed = DemoData.Initial();
        var first = AppealFeed.Fetch(feed, 3);
        feed.Add(DemoData.NewAppeal());
        var second = AppealFeed.Fetch(feed, 3, first.Next);

        Console.WriteLine("page1=" + string.Join(",", first.Items.Select(x => x.Id)));
        Console.WriteLine("inserted=107");
        Console.WriteLine("page2=" + string.Join(",", second.Items.Select(x => x.Id)));
        var overlap = first.Items.Select(x => x.Id).Intersect(second.Items.Select(x => x.Id)).ToArray();
        Console.WriteLine("overlap=" + string.Join(",", overlap));
        Console.WriteLine("original_seen=" +
            string.Join(",", first.Items.Concat(second.Items).Select(x => x.Id).Distinct().OrderBy(x => x)));

        if (args.Contains("--reproduce", StringComparer.Ordinal))
            return overlap.Length > 0 ? 0 : 1;

        return 0;
    }
}
