using System;
using System.Threading.Tasks;

internal sealed record Snapshot(string CenterId, int AvailableSeats);

internal sealed class OccupancyCache
{
    private readonly Task _gracePeriod;
    private readonly TaskCompletionSource<bool> _initialized =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private Snapshot? _snapshot;

    public OccupancyCache(Task gracePeriod) => _gracePeriod = gracePeriod;

    public async Task InitializeAsync(Task partnerReady)
    {
        try
        {
            await partnerReady.ConfigureAwait(false);
            _snapshot = new Snapshot("CENTER-17", 37);
            _initialized.TrySetResult(true);
        }
        catch (Exception ex)
        {
            _initialized.TrySetException(ex);
            throw;
        }
    }

    public async Task<Snapshot?> ReadAsync()
    {
        await _initialized.Task.ConfigureAwait(false);
        return _snapshot;
    }
}

internal static class Program
{
    private static TaskCompletionSource<bool> Gate() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static async Task<(bool Early, Snapshot? Value)> SlowAsync()
    {
        var grace = Gate();
        var partner = Gate();
        var cache = new OccupancyCache(grace.Task);
        Task startup = cache.InitializeAsync(partner.Task);
        grace.SetResult(true);
        Task<Snapshot?> read = cache.ReadAsync();
        bool early = read.IsCompleted;
        partner.SetResult(true);
        await startup.ConfigureAwait(false);
        return (early, await read.ConfigureAwait(false));
    }

    private static async Task<bool> FastAsync()
    {
        var grace = Gate();
        var partner = Gate();
        var cache = new OccupancyCache(grace.Task);
        partner.SetResult(true);
        await cache.InitializeAsync(partner.Task).ConfigureAwait(false);
        grace.SetResult(true);
        return await cache.ReadAsync().ConfigureAwait(false)
            is { CenterId: "CENTER-17", AvailableSeats: 37 };
    }

    private static async Task<bool> FaultAsync()
    {
        var grace = Gate();
        var partner = Gate();
        var cache = new OccupancyCache(grace.Task);
        Task startup = cache.InitializeAsync(partner.Task);
        grace.SetResult(true);
        Task<Snapshot?> read = cache.ReadAsync();
        partner.SetException(new InvalidOperationException("partner-snapshot-unavailable"));
        try { await startup.ConfigureAwait(false); }
        catch (InvalidOperationException) { }
        try { _ = await read.ConfigureAwait(false); return false; }
        catch (InvalidOperationException ex)
        {
            return ex.Message == "partner-snapshot-unavailable";
        }
    }

    public static async Task<int> Main(string[] args)
    {
        if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
        {
            Console.Error.WriteLine("Usage: reproduce|verify");
            return 2;
        }
        var slow = await SlowAsync().ConfigureAwait(false);
        bool fast = await FastAsync().ConfigureAwait(false);
        bool fault = await FaultAsync().ConfigureAwait(false);
        bool correct = slow.Value is { CenterId: "CENTER-17", AvailableSeats: 37 };
        Console.WriteLine($"slow: returned-before-ready={slow.Early}; seats={(slow.Value?.AvailableSeats.ToString() ?? "null")}");
        Console.WriteLine($"fast: correct={fast}");
        Console.WriteLine($"failed-partner: failure-propagated={fault}");
        if (args[0] == "reproduce")
        {
            bool symptom = slow.Early && !correct && fast && !fault;
            Console.WriteLine($"starter-symptom-confirmed={symptom}");
            return symptom ? 0 : 1;
        }
        bool pass = !slow.Early && correct && fast && fault;
        Console.WriteLine($"verification-passed={pass}");
        return pass ? 0 : 1;
    }
}
