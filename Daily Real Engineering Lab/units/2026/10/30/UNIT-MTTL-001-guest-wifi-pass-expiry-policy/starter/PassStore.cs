namespace VisitorPassLab;

public sealed record AccessPass(
    string Id,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset? RevokedAtUtc);

public sealed class PassStore
{
    private readonly Dictionary<string, AccessPass> _documents =
        new(StringComparer.Ordinal);

    public void Upsert(AccessPass pass) => _documents[pass.Id] = pass;

    public AccessPass? Find(string id) =>
        _documents.TryGetValue(id, out var pass) ? pass : null;

    // Mô phỏng một lượt cleanup bất đồng bộ: không chạy trong mỗi lần kiểm tra.
    public void RunCleanup(DateTimeOffset nowUtc)
    {
        var expiredIds = _documents.Values
            .Where(pass => pass.ExpiresAtUtc <= nowUtc)
            .Select(pass => pass.Id)
            .ToArray();

        foreach (var id in expiredIds)
            _documents.Remove(id);
    }
}
