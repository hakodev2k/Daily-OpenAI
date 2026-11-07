var store = new SessionStore("v1", version: 1, expiresAtStep: 2);

Console.WriteLine($"START {store.Describe()}");
var expiryCandidateVersion = store.Version;
Console.WriteLine($"EXPIRY_SCAN version={expiryCandidateVersion}");

store.Refresh("v2", 10);
Console.WriteLine($"REFRESH_ATOMIC {store.Describe()}");

var deleted = store.DeleteIfVersion(expiryCandidateVersion);
Console.WriteLine($"EXPIRY_DELETE accepted={deleted} candidateVersion={expiryCandidateVersion}");
Console.WriteLine($"FINAL {store.Describe()}");

sealed class SessionStore
{
    public string? Value { get; private set; }
    public int Version { get; private set; }
    public int ExpiresAtStep { get; private set; }

    public SessionStore(string value, int version, int expiresAtStep)
    {
        Value = value;
        Version = version;
        ExpiresAtStep = expiresAtStep;
    }

    public void Refresh(string value, int expiresAtStep)
    {
        Value = value;
        ExpiresAtStep = expiresAtStep;
        Version++;
    }

    public bool DeleteIfVersion(int expectedVersion)
    {
        if (Version != expectedVersion) return false;
        Value = null;
        return true;
    }

    public string Describe() => Value is null
        ? $"missing version={Version}"
        : $"value={Value} version={Version} expiresAt={ExpiresAtStep}";
}