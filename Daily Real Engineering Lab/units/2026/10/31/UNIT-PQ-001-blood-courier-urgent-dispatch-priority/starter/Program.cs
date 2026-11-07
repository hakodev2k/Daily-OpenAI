using DispatchLab;

var cases = new (string Name, Action Check)[]
{
    ("urgent escalation changes dispatch order", () =>
    {
        var q = new DispatchQueue();
        q.Register("BLD-A", 5);
        q.Register("BLD-B", 2);
        Require(q.Escalate("BLD-A", 0), "Escalation should be accepted.");
        Equal("BLD-A", q.DispatchNext(), "First dispatch");
        Equal("BLD-B", q.DispatchNext(), "Second dispatch");
        Equal(null, q.DispatchNext(), "Queue must be empty.");
        Equal(0, q.PendingCount, "Pending count");
    }),
    ("multiple escalations still dispatch exactly once", () =>
    {
        var q = new DispatchQueue();
        q.Register("BLD-C", 7);
        q.Register("BLD-D", 3);
        Require(q.Escalate("BLD-C", 2), "First escalation.");
        Require(q.Escalate("BLD-C", 0), "Second escalation.");
        Equal("BLD-C", q.DispatchNext(), "Latest priority first");
        Equal("BLD-D", q.DispatchNext(), "Remaining request");
        Equal(null, q.DispatchNext(), "No duplicate dispatch.");
        Equal(0, q.PendingCount, "Pending count");
    }),
    ("equal urgency retains admission order", () =>
    {
        var q = new DispatchQueue();
        q.Register("BLD-E", 3);
        q.Register("BLD-F", 3);
        Equal("BLD-E", q.DispatchNext(), "FIFO first");
        Equal("BLD-F", q.DispatchNext(), "FIFO second");
        Equal(null, q.DispatchNext(), "Empty after two.");
    }),
    ("unknown and already dispatched requests cannot escalate", () =>
    {
        var q = new DispatchQueue();
        Require(!q.Escalate("MISSING", 0), "Unknown request.");
        q.Register("BLD-G", 2);
        Equal("BLD-G", q.DispatchNext(), "Dispatch existing.");
        Require(!q.Escalate("BLD-G", 0), "Already dispatched request.");
        Equal(null, q.DispatchNext(), "No phantom request.");
    }),
    ("duplicate admission is rejected without changing pending work", () =>
    {
        var q = new DispatchQueue();
        q.Register("BLD-H", 4);
        try
        {
            q.Register("BLD-H", 0);
            throw new InvalidOperationException("Duplicate request was accepted.");
        }
        catch (ArgumentException)
        {
            // Expected rejection.
        }
        Equal(1, q.PendingCount, "One pending request.");
        Equal("BLD-H", q.DispatchNext(), "Original remains.");
    })
};

var failed = 0;
foreach (var test in cases)
{
    try
    {
        test.Check();
        Console.WriteLine($"PASS: {test.Name}");
    }
    catch (Exception ex)
    {
        failed++;
        Console.WriteLine($"FAIL: {test.Name} | {ex.GetType().Name}: {ex.Message}");
    }
}

Console.WriteLine(failed == 0 ? "LAB_RESULT:PASS" : $"LAB_RESULT:FAIL ({failed}/{cases.Length})");
return failed == 0 ? 0 : 1;

static void Require(bool condition, string label)
{
    if (!condition)
        throw new InvalidOperationException(label);
}

static void Equal<T>(T expected, T actual, string label)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new InvalidOperationException($"{label}: expected={expected ?? (object)"<null>"}, actual={actual ?? (object)"<null>"}");
}
