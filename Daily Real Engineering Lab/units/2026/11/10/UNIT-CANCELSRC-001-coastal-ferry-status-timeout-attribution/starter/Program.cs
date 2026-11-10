using System;
using System.Threading;
using System.Threading.Tasks;
var cases = new (string Name, bool CallerCancels, int UpstreamMs, int BudgetMs, string Expected)[]
{
    ("upstream-exceeds-budget", false, 120, 25, "UPSTREAM_TIMEOUT"),
    ("caller-disconnects", true, 120, 200, "CLIENT_CANCELLED"),
    ("healthy-upstream", false, 5, 200, "OK")
};
var mismatches = 0;
foreach (var test in cases)
{
    using var caller = new CancellationTokenSource();
    if (test.CallerCancels) caller.CancelAfter(20);
    var actual = await FerryStatusService.GetEtaStatusAsync(test.UpstreamMs, test.BudgetMs, caller.Token);
    if (actual != test.Expected) mismatches++;
    Console.WriteLine($"{test.Name}: actual={actual}; expected={test.Expected}");
}
if (args.Contains("--reproduce"))
{
    if (mismatches == 0) { Console.Error.WriteLine("No discrepancy reproduced"); return 1; }
    Console.WriteLine($"Reproduced {mismatches} discrepancies"); return 0;
}
if (args.Contains("--verify"))
{
    if (mismatches != 0) { Console.Error.WriteLine($"Failed {mismatches} contracts"); return 1; }
    Console.WriteLine("All contracts pass"); return 0;
}
return 0;
internal static class FerryStatusService
{
    public static async Task<string> GetEtaStatusAsync(int upstreamDelayMs, int budgetMs, CancellationToken callerToken)
    {
        using var budget = new CancellationTokenSource(TimeSpan.FromMilliseconds(budgetMs));
        using var combined = CancellationTokenSource.CreateLinkedTokenSource(callerToken, budget.Token);
        try { await Task.Delay(upstreamDelayMs, combined.Token); return "OK"; }
        catch (OperationCanceledException) { return "CLIENT_CANCELLED"; }
    }
}
