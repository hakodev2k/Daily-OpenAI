var store = new SessionStore("v1", version: 1, expiresAtStep: 2);

Console.WriteLine($"START {store.Describe()}");

// Actor A begins a refresh using the state it observed at step 1.
var observedVersion = store.Version;
Console.WriteLine($"REFRESH_READ version={observedVersion}");

// Actor B scans an entry that was eligible for expiry from its earlier snapshot.
var expiryCandidateVersion = store.Version;
Console.WriteLine($"EXPIRY_SCAN version={expiryCandidateVersion}");

// Actor A updates payload and TTL as separate operations.
store.SetValue("v2");
Console.WriteLine($"REFRESH_WRITE {store.Describe()}");
store.SetExpiry(10);
Console.WriteLine($"REFRESH_TTL {store.Describe()}");

// Actor B applies the deletion decision made from its earlier observation.
store.Delete();
Console.WriteLine($"EXPIRY_DELETE candidateVersion={expiryCandidateVersion}");

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

    public void SetValue(string value)
    {
        Value = value;
        Version++;
    }

    public void SetExpiry(int expiresAtStep) => ExpiresAtStep = expiresAtStep;

    public void Delete() => Value = null;

    public string Describe() => Value is null
        ? $"missing version={Version}"
        : $"value={Value} version={Version} expiresAt={ExpiresAtStep}";
}