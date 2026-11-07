using System.Threading.Tasks.Sources;

var source = new InventorySource();
var operation = source.ReadAsync();
var first = await operation;
Console.WriteLine($"inventory={first}");
if (args.Contains("audit"))
{
    var auditValue = await operation;
    Console.WriteLine($"audit={auditValue}");
}

sealed class InventorySource : IValueTaskSource<int>
{
    private ManualResetValueTaskSourceCore<int> _core;
    public InventorySource() { _core.RunContinuationsAsynchronously = true; }
    public ValueTask<int> ReadAsync()
    {
        _core.Reset();
        _core.SetResult(42);
        return new ValueTask<int>(this, _core.Version);
    }
    public int GetResult(short token) => _core.GetResult(token);
    public ValueTaskSourceStatus GetStatus(short token) => _core.GetStatus(token);
    public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags) => _core.OnCompleted(continuation, state, token, flags);
}