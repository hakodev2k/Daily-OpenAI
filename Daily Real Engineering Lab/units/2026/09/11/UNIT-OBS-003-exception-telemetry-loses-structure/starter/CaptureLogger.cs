using Microsoft.Extensions.Logging;

public sealed record CapturedLog(LogLevel Level, string Message, Exception? Exception);

public sealed class CaptureLogger<T> : ILogger<T>
{
    public CapturedLog? LastEntry { get; private set; }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        LastEntry = new CapturedLog(logLevel, formatter(state, exception), exception);
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        public void Dispose() { }
    }
}