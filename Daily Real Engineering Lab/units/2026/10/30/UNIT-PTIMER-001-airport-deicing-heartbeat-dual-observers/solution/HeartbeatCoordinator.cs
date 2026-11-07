using System.Threading.Channels;
namespace DeicingReadiness;
// Reference only; not compiled by Lab.csproj.
public sealed class HeartbeatCoordinator : IAsyncDisposable
{
    private readonly object _gate = new();
    private readonly PeriodicTimer _clock;
    private readonly List<ChannelWriter<long>> _subscribers = new();
    private Task? _pump;
    private bool _disposed;
    private long _sequence;
    public HeartbeatCoordinator(TimeSpan interval) => _clock = new PeriodicTimer(interval);
    public async Task<long[]> ObserveAsync(int count, CancellationToken cancellationToken = default)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        cancellationToken.ThrowIfCancellationRequested();
        var channel = Channel.CreateUnbounded<long>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _subscribers.Add(channel.Writer);
            _pump ??= Task.Run(PumpAsync);
        }
        try
        {
            var samples = new long[count];
            for (var i = 0; i < count; i++) samples[i] = await channel.Reader.ReadAsync(cancellationToken);
            return samples;
        }
        finally
        {
            lock (_gate) _subscribers.Remove(channel.Writer);
            channel.Writer.TryComplete();
        }
    }
    private async Task PumpAsync()
    {
        try
        {
            while (await _clock.WaitForNextTickAsync())
            {
                var tick = Interlocked.Increment(ref _sequence);
                lock (_gate)
                    foreach (var writer in _subscribers) writer.TryWrite(tick);
            }
        }
        finally
        {
            lock (_gate)
            {
                foreach (var writer in _subscribers) writer.TryComplete();
                _subscribers.Clear();
            }
        }
    }
    public async ValueTask DisposeAsync()
    {
        Task? pump;
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            _clock.Dispose();
            pump = _pump;
        }
        if (pump is not null) await pump;
    }
}
