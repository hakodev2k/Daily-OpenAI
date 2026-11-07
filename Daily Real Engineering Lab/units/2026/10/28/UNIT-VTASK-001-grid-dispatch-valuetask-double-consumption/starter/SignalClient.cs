using System.Threading;
using System.Threading.Tasks.Sources;

namespace GridSignalLab;

public sealed class SimulatedSignalClient
{
    private int _requestCount;
    public int RequestCount => Volatile.Read(ref _requestCount);

    public ValueTask<int> ReadValidatedKilowattsAsync(string signalId)
    {
        Interlocked.Increment(ref _requestCount);
        int kw = signalId switch
        {
            "GRID-NORTH-11" => 42,
            "GRID-SOUTH-12" => 67,
            _ => throw new ArgumentOutOfRangeException(nameof(signalId), signalId, "Unknown signal")
        };
        return new ValueTask<int>(new SingleUseReadingSource(kw), token: 0);
    }
}

// Mô phỏng một provider trả về kết quả đã hoàn thành nhưng có ownership một lần.
// Không chỉnh sửa fixture này khi giải lab.
internal sealed class SingleUseReadingSource : IValueTaskSource<int>
{
    private readonly int _value;
    private int _consumed;

    public SingleUseReadingSource(int value) => _value = value;

    public int GetResult(short token)
    {
        ValidateToken(token);
        if (Interlocked.Exchange(ref _consumed, 1) != 0)
            throw new InvalidOperationException("Signal reading already consumed");
        return _value;
    }

    public ValueTaskSourceStatus GetStatus(short token)
    {
        ValidateToken(token);
        return ValueTaskSourceStatus.Succeeded;
    }

    public void OnCompleted(Action<object?> continuation, object? state, short token,
        ValueTaskSourceOnCompletedFlags flags)
    {
        ValidateToken(token);
        throw new InvalidOperationException("This fixture is already completed");
    }

    private static void ValidateToken(short token)
    {
        if (token != 0) throw new InvalidOperationException("Unexpected source token");
    }
}
