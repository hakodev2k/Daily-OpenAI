namespace IntakeLab;

public sealed class ManifestStore
{
    private readonly object _gate = new();
    private Dictionary<string, int> _rows = new(StringComparer.Ordinal);

    public int Count
    {
        get { lock (_gate) return _rows.Count; }
    }

    public IReadOnlyDictionary<string, int> Snapshot()
    {
        lock (_gate) return new Dictionary<string, int>(_rows, StringComparer.Ordinal);
    }

    public Task UpsertAsync(IntakeRow row, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate) _rows[row.Id] = row.Qty;
        return Task.CompletedTask;
    }

    // Store primitive for one indivisible batch publication in this local simulation.
    public Task ApplyBatchAsync(
        IReadOnlyList<IntakeRow> rows,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        lock (_gate)
        {
            var next = new Dictionary<string, int>(_rows, StringComparer.Ordinal);
            foreach (var row in rows)
            {
                cancellationToken.ThrowIfCancellationRequested();
                next[row.Id] = row.Qty;
            }

            cancellationToken.ThrowIfCancellationRequested();
            _rows = next;
        }

        return Task.CompletedTask;
    }
}
