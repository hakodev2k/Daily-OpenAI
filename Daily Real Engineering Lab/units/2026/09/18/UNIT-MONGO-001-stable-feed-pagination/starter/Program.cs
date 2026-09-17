record FeedItem(int Id, DateTimeOffset CreatedAt);

var items = new List<FeedItem>
{
    new(5, DateTimeOffset.Parse("2026-09-18T00:05:00+07:00")),
    new(4, DateTimeOffset.Parse("2026-09-18T00:04:00+07:00")),
    new(3, DateTimeOffset.Parse("2026-09-18T00:03:00+07:00")),
    new(2, DateTimeOffset.Parse("2026-09-18T00:02:00+07:00")),
    new(1, DateTimeOffset.Parse("2026-09-18T00:01:00+07:00"))
};

static IReadOnlyList<FeedItem> GetPage(List<FeedItem> source, int page, int size)
{
    // Investigation note: what assumption does page number make about a mutable ordered dataset?
    return source.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
        .Skip((page - 1) * size).Take(size).ToList();
}

var first = GetPage(items, 1, 2);
items.Add(new FeedItem(6, DateTimeOffset.Parse("2026-09-18T00:06:00+07:00")));
var second = GetPage(items, 2, 2);

Console.WriteLine($"Page1: {string.Join(',', first.Select(x => x.Id))}");
Console.WriteLine($"Page2: {string.Join(',', second.Select(x => x.Id))}");
var overlap = first.Select(x => x.Id).Intersect(second.Select(x => x.Id)).ToArray();
Console.WriteLine(overlap.Length == 0 ? "TRAVERSAL_OK" : $"TRAVERSAL_BROKEN overlap={string.Join(',', overlap)}");
Environment.ExitCode = overlap.Length == 0 ? 0 : 2;