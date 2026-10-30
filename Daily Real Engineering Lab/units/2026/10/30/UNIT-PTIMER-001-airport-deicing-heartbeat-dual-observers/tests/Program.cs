using DeicingReadiness;
internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var mode = args.Length == 1 ? args[0] : "verify";
        if (mode is not ("reproduce" or "verify")) { Console.Error.WriteLine("Use reproduce or verify"); return 2; }
        var cases = new (string, Func<Task>)[]
        {
            ("dual-observer-same-sequence", DualObservers),
            ("cancel-one-observer-isolated", CancellationIsolation)
        };
        var errors = new List<Exception>();
        foreach (var (name, test) in cases)
        {
            try { await test(); Console.WriteLine($"PASS {name}"); }
            catch (Exception ex) { errors.Add(ex); Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}"); }
        }
        if (mode == "reproduce")
        {
            var expected = errors.Count == 2 && errors.All(x => x is InvalidOperationException);
            Console.WriteLine(expected ? "BASELINE REPRODUCED" : "BASELINE NOT REPRODUCED");
            return expected ? 0 : 1;
        }
        if (mode == "verify")
        {
            Console.WriteLine(errors.Count == 0 ? "ALL CONTRACTS PASS" : "VERIFICATION FAILED");
            return errors.Count == 0 ? 0 : 1;
        }
        return 2;
    }
    private static async Task DualObservers()
    {
        await using var hub = new HeartbeatCoordinator(TimeSpan.FromMilliseconds(80));
        var a = hub.ObserveAsync(3);
        var b = hub.ObserveAsync(3);
        var result = await Task.WhenAll(a, b).WaitAsync(TimeSpan.FromSeconds(4));
        if (result.Any(x => x.Length != 3) || !result[0].SequenceEqual(result[1]) || !Increasing(result[0]))
            throw new Exception("Both observers must receive the same three ordered heartbeat IDs.");
    }
    private static async Task CancellationIsolation()
    {
        await using var hub = new HeartbeatCoordinator(TimeSpan.FromMilliseconds(80));
        using var cts = new CancellationTokenSource();
        var canceled = hub.ObserveAsync(6, cts.Token);
        var survivor = hub.ObserveAsync(3);
        cts.CancelAfter(TimeSpan.FromMilliseconds(120));
        try { await canceled.WaitAsync(TimeSpan.FromSeconds(4)); throw new Exception("Canceled observer completed unexpectedly."); }
        catch (OperationCanceledException) when (cts.IsCancellationRequested) { }
        var result = await survivor.WaitAsync(TimeSpan.FromSeconds(4));
        if (result.Length != 3 || !Increasing(result)) throw new Exception("Other observer did not receive three ordered heartbeats.");
    }
    private static bool Increasing(long[] a)
    {
        if (a.Length == 0) return false;
        for (int i = 1; i < a.Length; i++) if (a[i] <= a[i - 1]) return false;
        return true;
    }
}
