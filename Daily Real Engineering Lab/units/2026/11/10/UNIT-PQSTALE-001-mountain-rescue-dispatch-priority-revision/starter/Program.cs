using RescueLab;

if (args.Contains("--contract"))
{
    var failures = new List<string>();
    void Check(string name, Action test)
    {
        try { test(); Console.WriteLine($"PASS {name}"); }
        catch (Exception ex) { failures.Add(name); Console.WriteLine($"FAIL {name}: {ex.Message}"); }
    }

    Check("severity revision changes dispatch order", () =>
    {
        var q = new DispatchQueue();
        q.Enqueue("ROUTINE", 4);
        q.Enqueue("CRITICAL", 8);
        q.Revise("CRITICAL", 1);
        if (q.DispatchNext()?.IncidentId != "CRITICAL") throw new Exception("Upgraded incident dispatched late");
        if (q.DispatchNext()?.IncidentId != "ROUTINE") throw new Exception("Routine incident lost");
    });
    Check("revision may lower priority", () =>
    {
        var q = new DispatchQueue();
        q.Enqueue("A", 1); q.Enqueue("B", 3); q.Revise("A", 8);
        if (q.DispatchNext()?.IncidentId != "B") throw new Exception("Downgraded incident dispatched early");
    });
    Check("repeated revisions never dispatch duplicates", () =>
    {
        var q = new DispatchQueue();
        q.Enqueue("A", 5); q.Enqueue("B", 4);
        q.Revise("A", 2); q.Revise("A", 1); q.Revise("A", 6);
        if (q.PendingCount != 2) throw new Exception("Logical pending count changed");
        if (q.DispatchNext()?.IncidentId != "B" || q.DispatchNext()?.IncidentId != "A" || q.DispatchNext() is not null)
            throw new Exception("Incorrect dispatch or stale duplicate");
    });
    Check("unknown incident revision rejected", () =>
    {
        var q = new DispatchQueue();
        try { q.Revise("MISSING", 1); throw new Exception("Expected rejection"); }
        catch (KeyNotFoundException) { }
    });
    Check("duplicate incident rejected", () =>
    {
        var q = new DispatchQueue(); q.Enqueue("A", 3);
        try { q.Enqueue("A", 2); throw new Exception("Expected rejection"); }
        catch (InvalidOperationException ex) when (ex.Message == "Duplicate incident") { }
    });
    Check("severity validation", () =>
    {
        var q = new DispatchQueue();
        try { q.Enqueue("A", 0); throw new Exception("Expected validation"); }
        catch (ArgumentOutOfRangeException) { }
    });
    Console.WriteLine($"RESULT failures={failures.Count}");
    return failures.Count == 0 ? 0 : 1;
}

var queue = new DispatchQueue();
queue.Enqueue("ROUTINE", 4);
queue.Enqueue("CRITICAL", 8);
Console.WriteLine("02:00 accepted ROUTINE severity=4");
Console.WriteLine("02:01 accepted CRITICAL severity=8");
queue.Revise("CRITICAL", 1);
Console.WriteLine("02:02 revised CRITICAL severity=1");
Console.WriteLine($"02:03 dispatched {queue.DispatchNext()}");
Console.WriteLine($"02:04 dispatched {queue.DispatchNext()}");
return 0;
