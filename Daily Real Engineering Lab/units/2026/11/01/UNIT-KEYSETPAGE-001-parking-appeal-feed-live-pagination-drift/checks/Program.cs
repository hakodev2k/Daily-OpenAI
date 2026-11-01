public static class ContractChecks
{
    public static int Main()
    {
        try
        {
            StaticFeed();
            InsertBetweenPages();
            DeleteBeforeCursor();
            EqualTimestampTie();
            EmptyAndInvalidPageSize();
            Console.WriteLine("PASS: static, insert, delete, equal timestamp, empty and invalid-size contracts");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("FAIL: " + ex.Message);
            return 1;
        }
    }

    private static void StaticFeed()
    {
        var feed = DemoData.Initial();
        var first = AppealFeed.Fetch(feed, 3);
        AssertIds(first.Items, 106, 105, 104);
        Assert(first.Next is not null, "Trang 1 phải có continuation.");
        Assert(first.Next!.LastCreatedAt == first.Items[^1].CreatedAt &&
               first.Next.LastId == first.Items[^1].Id,
               "Continuation phải mang sort key của bản ghi cuối trang.");
        var second = AppealFeed.Fetch(feed, 3, first.Next);
        AssertIds(second.Items, 103, 102, 101);
        Assert(second.Next is null, "Trang cuối không được quảng cáo continuation rỗng.");
    }

    private static void InsertBetweenPages()
    {
        var feed = DemoData.Initial();
        var first = AppealFeed.Fetch(feed, 3);
        feed.Add(DemoData.NewAppeal());
        var second = AppealFeed.Fetch(feed, 3, first.Next);
        AssertIds(second.Items, 103, 102, 101);
        Assert(!second.Items.Any(x => x.Id == 107), "Hồ sơ mới không được chen vào sau cursor cũ.");
        Assert(!first.Items.Select(x => x.Id).Intersect(second.Items.Select(x => x.Id)).Any(),
            "Một hồ sơ không được xuất hiện hai lần.");
        Assert(first.Items.Concat(second.Items).Select(x => x.Id).OrderBy(x => x)
            .SequenceEqual(Enumerable.Range(101, 6).Select(x => (long)x)),
            "Phải thấy đủ sáu hồ sơ ban đầu.");
    }

    private static void DeleteBeforeCursor()
    {
        var feed = DemoData.Initial();
        var first = AppealFeed.Fetch(feed, 3);
        feed.RemoveAll(x => x.Id == 106);
        var second = AppealFeed.Fetch(feed, 3, first.Next);
        AssertIds(second.Items, 103, 102, 101);
    }

    private static void EqualTimestampTie()
    {
        var t = DemoData.BaseTime;
        var feed = new List<Appeal>
        {
            new(205, t.AddMinutes(1), "PENDING"),
            new(204, t, "PENDING"),
            new(203, t, "PENDING"),
            new(202, t.AddMinutes(-1), "PENDING"),
            new(201, t.AddMinutes(-2), "PENDING")
        };
        var first = AppealFeed.Fetch(feed, 2);
        AssertIds(first.Items, 205, 204);
        Assert(first.Next is not null, "Trang tie đầu cần continuation.");
        feed.Add(new Appeal(206, t, "PENDING"));
        var second = AppealFeed.Fetch(feed, 2, first.Next);
        AssertIds(second.Items, 203, 202);
        Assert(second.Next is not null, "Trang tie thứ hai cần continuation.");
        var third = AppealFeed.Fetch(feed, 2, second.Next);
        AssertIds(third.Items, 201);
        Assert(third.Next is null, "Trang cuối phải kết thúc.");
    }

    private static void EmptyAndInvalidPageSize()
    {
        var empty = AppealFeed.Fetch(Array.Empty<Appeal>(), 3);
        Assert(empty.Items.Count == 0 && empty.Next is null, "Feed rỗng phải kết thúc.");
        ExpectOutOfRange(() => AppealFeed.Fetch(DemoData.Initial(), 0));
        ExpectOutOfRange(() => AppealFeed.Fetch(DemoData.Initial(), 101));
    }

    private static void ExpectOutOfRange(Action action)
    {
        try { action(); }
        catch (ArgumentOutOfRangeException) { return; }
        throw new Exception("Page size ngoài 1..100 phải bị từ chối.");
    }

    private static void AssertIds(IReadOnlyList<Appeal> actual, params long[] expected)
    {
        if (!actual.Select(x => x.Id).SequenceEqual(expected))
            throw new Exception("Expected [" + string.Join(",", expected) +
                "] but got [" + string.Join(",", actual.Select(x => x.Id)) + "]");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}
