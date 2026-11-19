using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var gate = new PartnerGateway(2);
        var results = new List<string>();
        foreach (var item in new[] { "BAG-101", "BAG-FAIL-1", "BAG-102", "BAG-FAIL-2" })
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));
            try { results.Add(await gate.ReconcileAsync(item, timeout.Token)); }
            catch (Exception ex) when (ex is InvalidOperationException or OperationCanceledException)
            { results.Add(ex is OperationCanceledException ? "TIMEOUT" : "PARTNER_ERROR"); }
        }
        Console.WriteLine("results=" + string.Join(",", results));
        Console.WriteLine("availablePermits=" + gate.AvailablePermits);
        bool healthy = results.SequenceEqual(new[] { "OK", "PARTNER_ERROR", "OK", "PARTNER_ERROR" })
                       && gate.AvailablePermits == 2;
        if (args.Contains("--verify")) { Console.WriteLine(healthy ? "PASS" : "FAIL"); return healthy ? 0 : 1; }
        return 0;
    }
}

internal sealed class PartnerGateway
{
    private readonly SemaphoreSlim _capacity;
    public PartnerGateway(int concurrency) => _capacity = new SemaphoreSlim(concurrency, concurrency);
    public int AvailablePermits => _capacity.CurrentCount;

    public async Task<string> ReconcileAsync(string baggageId, CancellationToken cancellationToken)
    {
        await _capacity.WaitAsync(cancellationToken);
        // Partner responses are deterministic in this local replay.
        await Task.Delay(10, cancellationToken);
        if (baggageId.Contains("FAIL", StringComparison.Ordinal))
            throw new InvalidOperationException("Partner HTTP 503");
        _capacity.Release();
        return "OK";
    }
}
