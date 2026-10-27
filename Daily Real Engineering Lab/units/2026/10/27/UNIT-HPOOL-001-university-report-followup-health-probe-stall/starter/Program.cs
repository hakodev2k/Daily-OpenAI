if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [reproduce|verify]");
    return 2;
}
var mode = args[0];
await using var partner = new LocalPartnerServer();
using var overallDeadline = new CancellationTokenSource(TimeSpan.FromSeconds(6));
try
{
    var result = await new PartnerClient().ProbeAsync(partner.BaseUri, overallDeadline.Token);
    if (mode == "reproduce")
    {
        Console.Error.WriteLine($"UNEXPECTED: symptom absent; result={result}, reports={partner.ReportRequests}, health={partner.HealthRequests}");
        return 1;
    }
    if (result == "PONG" && partner.ReportRequests == 1 && partner.HealthRequests == 1)
    {
        Console.WriteLine("PASS: report headers accepted; health reached origin; response=PONG.");
        return 0;
    }
    Console.Error.WriteLine($"FAIL: contract mismatch; result={result}, reports={partner.ReportRequests}, health={partner.HealthRequests}");
    return 1;
}
catch (OperationCanceledException ex)
{
    if (mode == "reproduce" && !overallDeadline.IsCancellationRequested &&
        partner.ReportRequests == 1 && partner.HealthRequests == 0)
    {
        Console.WriteLine("EXPECTED FAILURE: health probe timed out before reaching partner.");
        Console.WriteLine($"evidence: reports={partner.ReportRequests}, health={partner.HealthRequests}, exception={ex.GetType().Name}");
        return 0;
    }
    Console.Error.WriteLine($"FAIL: timeout; reports={partner.ReportRequests}, health={partner.HealthRequests}, overallExpired={overallDeadline.IsCancellationRequested}");
    return 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
    return 1;
}
