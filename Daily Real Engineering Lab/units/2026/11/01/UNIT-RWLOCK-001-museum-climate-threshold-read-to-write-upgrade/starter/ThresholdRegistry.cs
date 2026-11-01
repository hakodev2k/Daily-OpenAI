using System.Threading;

public sealed class ThresholdRegistry : IDisposable
{
    private readonly ReaderWriterLockSlim _gate = new();
    private readonly Dictionary<string, int> _thresholds = new(StringComparer.Ordinal)
    {
        ["STABLE"] = 8
    };
    private int _createdCount;

    public int CreatedCount => Volatile.Read(ref _createdCount);

    public int GetOrCreate(string sensorCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sensorCode);

        _gate.EnterReadLock();
        try
        {
            if (_thresholds.TryGetValue(sensorCode, out int existing))
                return existing;

            _gate.EnterWriteLock();
            try
            {
                if (_thresholds.TryGetValue(sensorCode, out existing))
                    return existing;

                Thread.Sleep(5); // Mô phỏng thao tác tạo bản ghi cấu hình.
                _thresholds.Add(sensorCode, 12);
                Interlocked.Increment(ref _createdCount);
                return 12;
            }
            finally
            {
                _gate.ExitWriteLock();
            }
        }
        finally
        {
            _gate.ExitReadLock();
        }
    }

    public void Dispose() => _gate.Dispose();
}
