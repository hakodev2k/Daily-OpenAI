using MarineSurveyLab;

var mode = args.Length == 0 ? "--demo" : args[0];

try
{
    switch (mode)
    {
        case "--demo":
        {
            var ids = Traverse(BaselinePager.FetchPage, Samples.All, 2);
            Console.WriteLine("SOURCE_COUNT=" + Samples.All.Count);
            Console.WriteLine("RECEIVED_COUNT=" + ids.Count);
            Console.WriteLine("RECEIVED_IDS=" + string.Join(",", ids));
            return 0;
        }
        case "--reproduce":
        {
            var ids = Traverse(BaselinePager.FetchPage, Samples.All, 2);
            var missing = Samples.All.Select(x => x.ObservationId)
                .Except(ids, StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToArray();
            Console.WriteLine("SOURCE_COUNT=" + Samples.All.Count);
            Console.WriteLine("RECEIVED_COUNT=" + ids.Count);
            Console.WriteLine("RECEIVED_IDS=" + string.Join(",", ids));
            Console.WriteLine("MISSING_IDS=" + string.Join(",", missing));
            if (ids.SequenceEqual(new[] { "obs-a", "obs-b", "obs-d", "obs-e", "obs-f" })
                && missing.SequenceEqual(new[] { "obs-c" }))
            {
                Console.WriteLine("REPRODUCED: missing=obs-c");
                return 0;
            }
            throw new InvalidOperationException("Baseline symptom differs from expected fixture.");
        }
        case "--verify":
        {
            var expected = new[] { "obs-a", "obs-b", "obs-c", "obs-d", "obs-e", "obs-f" };
            Check("page-size-1", Traverse(CursorPager.FetchPage, Samples.All, 1), expected);
            Check("page-size-2", Traverse(CursorPager.FetchPage, Samples.All, 2), expected);
            Check("page-size-3", Traverse(CursorPager.FetchPage, Samples.All, 3), expected);
            Check("shuffled-input", Traverse(CursorPager.FetchPage, Samples.All.Reverse().ToArray(), 2), expected);
            var empty = CursorPager.FetchPage(Array.Empty<Observation>(), null, 2);
            if (empty.Items.Count != 0 || empty.NextCursor is not null)
                throw new InvalidOperationException("Empty-source contract failed.");
            try
            {
                CursorPager.FetchPage(Samples.All, null, 0);
                throw new InvalidOperationException("Invalid pageSize must be rejected.");
            }
            catch (ArgumentOutOfRangeException) { }
            Console.WriteLine("CHECKS_PASSED=5");
            Console.WriteLine("VERIFIED: learner pager satisfies all regression contracts");
            return 0;
        }
        default:
            Console.Error.WriteLine("Usage: --demo | --reproduce | --verify");
            return 2;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine("CHECK_FAILED: " + ex.Message);
    return 1;
}

static List<string> Traverse(
    Func<IReadOnlyList<Observation>, string?, int, Page> fetch,
    IReadOnlyList<Observation> rows,
    int pageSize)
{
    var collected = new List<string>();
    string? cursor = null;
    for (var iteration = 0; iteration <= rows.Count + 1; iteration++)
    {
        var page = fetch(rows, cursor, pageSize);
        if (page.Items.Count == 0) return collected;
        if (page.Items.Count > pageSize) throw new InvalidOperationException("Page exceeds pageSize.");
        if (string.IsNullOrWhiteSpace(page.NextCursor))
            throw new InvalidOperationException("Nonempty page must have a continuation cursor.");
        collected.AddRange(page.Items.Select(x => x.ObservationId));
        cursor = page.NextCursor;
    }
    throw new InvalidOperationException("Cursor did not terminate.");
}

static void Check(string name, IReadOnlyList<string> actual, IReadOnlyList<string> expected)
{
    if (!actual.SequenceEqual(expected))
        throw new InvalidOperationException(name + ": expected " + string.Join(",", expected)
            + "; got " + string.Join(",", actual));
    Console.WriteLine("PASS " + name);
}
