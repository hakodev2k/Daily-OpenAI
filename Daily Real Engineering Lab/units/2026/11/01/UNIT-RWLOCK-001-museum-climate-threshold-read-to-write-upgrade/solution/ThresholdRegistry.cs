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

        // Fast path: cache hits do not contend for the single upgradeable slot.
        _gate.EnterReadLock();
        try
        {
            if (_thresholds.TryGetValue(sensorCode, out int found))
                return found;
        }
        finally
        {
            _gate.ExitReadLock();
        }

        _gate.EnterUpgradeableReadLock();
        try
        {
            // Another thread may have created this entry after our first read.
            if (_thresholds.TryGetValue(sensorCode, out int found))
                return found;

            _gate.EnterWriteLock();
            try
            {
                if (_thresholds.TryGetValue(sensorCode, out found))
                    return found;

                Thread.Sleep(5); // Unit-local deterministic creation cost.
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
            _gate.ExitUpgradeableReadLock();
        }
    }

    public void Dispose() => _gate.Dispose();
}
