using MuseumLoanAudit;

try
{
    foreach (int count in new[] { 0, 1, 9, 40 })
    {
        var events = Enumerable.Range(1, count)
            .Select(i => new AuditEvent(i, $"LOAN-{i:000}"))
            .ToArray();
        var result = await AuditPipeline.RunAsync(
            events, static (_, _) => ValueTask.CompletedTask);
        var expected = events.Select(e => e.Sequence).ToArray();
        if (result.Submitted != count || !result.StoredSequences.SequenceEqual(expected))
            throw new Exception($"LOSS_OR_ORDER: count={count}, submitted={result.Submitted}, stored=[{string.Join(",", result.StoredSequences)}]");
        Console.WriteLine($"PASS count={count} exact-once ordered audit records");
    }

    var failingBatch = Enumerable.Range(1, 9)
        .Select(i => new AuditEvent(i, $"FAIL-{i:000}")).ToArray();
    bool propagated = false;
    try
    {
        await AuditPipeline.RunAsync(failingBatch,
            static (e, _) => e.Sequence == 2
                ? ValueTask.FromException(new InvalidOperationException("sink-unavailable"))
                : ValueTask.CompletedTask);
    }
    catch (InvalidOperationException ex) when (ex.Message == "sink-unavailable")
    {
        propagated = true;
    }
    if (!propagated)
        throw new Exception("SINK_FAILURE_HIDDEN: persistence failure must reach caller.");

    Console.WriteLine("PASS persistence failure propagated");
    Console.WriteLine("VERIFIED: no audit loss, stable order, sink failure visible.");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"FAILED: {ex.Message}");
    return 1;
}
