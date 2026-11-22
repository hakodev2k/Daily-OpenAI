using System;
using System.Collections.Generic;
using System.Threading;

var audit = new List<string>();
if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [reproduce|verify]");
    return 2;
}
if (args[0] == "reproduce")
{
    using var shutdown = new CancellationTokenSource();
    for (int i = 0; i < 3; i++) CompleteScan(shutdown.Token, audit);
    Console.WriteLine($"Before shutdown: {audit.Count} cancellation audit entries");
    shutdown.Cancel();
    Console.WriteLine($"After shutdown: {audit.Count} cancellation audit entries (expected symptom: 3)");
    return audit.Count == 3 ? 0 : 1;
}
using (var shutdown = new CancellationTokenSource())
{
    for (int i = 0; i < 3; i++) CompleteScan(shutdown.Token, audit);
    shutdown.Cancel();
    if (audit.Count != 0)
    {
        Console.Error.WriteLine($"FAIL: {audit.Count} completed scans were incorrectly marked cancelled.");
        return 1;
    }
}
using (var inFlight = new CancellationTokenSource())
{
    CompleteScan(inFlight.Token, audit, () => inFlight.Cancel());
    if (audit.Count != 1)
    {
        Console.Error.WriteLine($"FAIL: cancellation during active work should be observed once; got {audit.Count}.");
        return 1;
    }
}
Console.WriteLine("PASS: completed operations have no late callbacks; in-flight cancellation is observed once.");
return 0;

static void CompleteScan(CancellationToken token, List<string> audit, Action? beforeCompletion = null)
{
    // A scan is accepted, processed, then acknowledged. Shutdown can happen during processing.
    token.Register(() => audit.Add("scan-cancelled"));
    beforeCompletion?.Invoke();
    // Persisted scan receipt is assumed successful at this boundary.
}
