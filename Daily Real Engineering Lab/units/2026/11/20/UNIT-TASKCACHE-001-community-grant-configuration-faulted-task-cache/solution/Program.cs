using System;
using System.Threading.Tasks;

var provider = new GrantPolicyProvider();
await RunAsync("initial", provider);
await RunAsync("recovery", provider);
return;

static async Task RunAsync(string label, GrantPolicyProvider provider)
{
    try { Console.WriteLine($"{label}: {await provider.GetPolicyAsync()}"); }
    catch (Exception ex) { Console.WriteLine($"{label}: {ex.GetType().Name}"); }
}

public sealed class GrantPolicyProvider
{
    private readonly object _gate = new();
    private Task<string>? _cached;
    private int _calls;

    public Task<string> GetPolicyAsync()
    {
        lock (_gate)
        {
            if (_cached is null || _cached.IsFaulted || _cached.IsCanceled)
                _cached = LoadAsync();
            return _cached;
        }
    }

    private async Task<string> LoadAsync()
    {
        await Task.Yield();
        if (System.Threading.Interlocked.Increment(ref _calls) == 1)
            throw new InvalidOperationException("Transient configuration endpoint failure");
        return "grant-policy:v2";
    }
}
