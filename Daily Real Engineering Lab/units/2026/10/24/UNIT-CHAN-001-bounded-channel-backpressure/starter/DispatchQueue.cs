using System.Threading.Channels;

namespace QueueLab;

public sealed class DispatchQueue : IAsyncDisposable
{
    private readonly Channel<int> _channel;
    private readonly Func<int, Task> _handler;
    private readonly Task _consumer;
    private long _accepted;
    private long _completed;
    private long _peakOutstanding;

    public long Accepted => Volatile.Read(ref _accepted);
    public long Completed => Volatile.Read(ref _completed);
    public long PeakOutstanding => Volatile.Read(ref _peakOutstanding);

    public DispatchQueue(Func<int, Task> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _channel = Channel.CreateUnbounded<int>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });
        _consumer = Task.Run(ConsumeAsync);
    }

    public async ValueTask EnqueueAsync(int item, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(item, cancellationToken);
        var outstanding = Interlocked.Increment(ref _accepted) - Volatile.Read(ref _completed);
        RecordPeak(outstanding);
    }

    public async Task CompleteAsync()
    {
        _channel.Writer.TryComplete();
        await _consumer;
    }

    private async Task ConsumeAsync()
    {
        await foreach (var item in _channel.Reader.ReadAllAsync())
        {
            await _handler(item);
            Interlocked.Increment(ref _completed);
        }
    }

    private void RecordPeak(long value)
    {
        long observed;
        while (value > (observed = Volatile.Read(ref _peakOutstanding)))
        {
            if (Interlocked.CompareExchange(ref _peakOutstanding, value, observed) == observed)
                break;
        }
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.TryComplete();
        await _consumer;
    }
}
