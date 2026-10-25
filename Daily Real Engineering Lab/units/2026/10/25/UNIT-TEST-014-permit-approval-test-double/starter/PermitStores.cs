namespace PermitLab;

public interface IPermitStore
{
    Task<PermitRecord?> FindAsync(string permitId);
    Task<PermitRecord?> ReloadAsync(string permitId);
    Task SaveAsync(PermitRecord record);
    int SaveCount { get; }
}

public abstract class PermitStoreBase : IPermitStore
{
    private readonly Dictionary<string, PermitRecord> _rows = new(StringComparer.Ordinal)
    {
        ["P-204"] = new PermitRecord("P-204", "Pending", 7)
    };

    public int SaveCount { get; private set; }

    protected PermitRecord? FindStored(string permitId) =>
        _rows.TryGetValue(permitId, out var record) ? record : null;

    // Default store returns a detached snapshot, similar to a repository without tracking.
    public virtual Task<PermitRecord?> FindAsync(string permitId) =>
        Task.FromResult(FindStored(permitId)?.Snapshot());

    // Always emulate a new read, not a cached response from the service.
    public Task<PermitRecord?> ReloadAsync(string permitId) =>
        Task.FromResult(FindStored(permitId)?.Snapshot());

    public Task SaveAsync(PermitRecord record)
    {
        if (!_rows.ContainsKey(record.PermitId))
            throw new InvalidOperationException("Permit does not exist.");

        _rows[record.PermitId] = record.Snapshot();
        SaveCount++;
        return Task.CompletedTask;
    }
}

// Existing unit-test fake. It looks convenient because callers can mutate the returned object.
public sealed class IdentityMapFakeStore : PermitStoreBase
{
    public override Task<PermitRecord?> FindAsync(string permitId) =>
        Task.FromResult(FindStored(permitId));
}

public sealed class DetachedPermitStore : PermitStoreBase
{
}
