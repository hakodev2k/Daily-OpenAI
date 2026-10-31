using MuseumLoanAudit;

string mode = args.Length > 0 ? args[0] : "demo";
if (mode is not ("demo" or "reproduce"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [demo|reproduce]");
    return 2;
}

var events = Enumerable.Range(1, 9)
    .Select(i => new AuditEvent(i, $"MUSEUM-LOAN-{i:000}"))
    .ToArray();
var result = await AuditPipeline.RunAsync(
    events,
    static (_, _) => ValueTask.CompletedTask);

Console.WriteLine($"submitted={result.Submitted} stored={result.StoredSequences.Count}");
Console.WriteLine($"storedSequences=[{string.Join(",", result.StoredSequences)}]");
var missing = events.Select(e => e.Sequence).Except(result.StoredSequences).ToArray();
Console.WriteLine($"missingSequences=[{string.Join(",", missing)}]");

if (mode == "reproduce")
{
    if (result.Submitted == events.Length && missing.Length > 0)
    {
        Console.WriteLine("REPRODUCED: accepted operations are missing from audit storage.");
        return 0;
    }
    Console.Error.WriteLine("NOT REPRODUCED: baseline loss was not observed.");
    return 1;
}
return 0;
