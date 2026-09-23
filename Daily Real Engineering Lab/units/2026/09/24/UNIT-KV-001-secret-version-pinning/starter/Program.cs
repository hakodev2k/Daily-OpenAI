var store = new SecretStore();
store.Add("payment-api-key", "v1", "credential-old", makeCurrent: true);

var configuredReference = "payment-api-key/v1";
var client = new PaymentCredentialProvider(store, configuredReference);

Console.WriteLine($"Store current before rotation: {store.CurrentVersion("payment-api-key")}");
Console.WriteLine($"Application credential before rotation: {client.GetCredential()}");

store.Add("payment-api-key", "v2", "credential-new", makeCurrent: true);

Console.WriteLine($"Store current after rotation: {store.CurrentVersion("payment-api-key")}");
Console.WriteLine($"Application credential after rotation: {client.GetCredential()}");

if (args.Contains("--verify"))
{
    if (client.GetCredential() != "credential-new")
        throw new InvalidOperationException("Rotation contract failed: application did not observe the current credential.");

    Console.WriteLine("VERIFY_PASS");
}

sealed class PaymentCredentialProvider
{
    private readonly SecretStore _store;
    private readonly string _reference;

    public PaymentCredentialProvider(SecretStore store, string reference)
    {
        _store = store;
        _reference = reference;
    }

    public string GetCredential() => _store.Resolve(_reference);
}

sealed class SecretStore
{
    private readonly Dictionary<string, Dictionary<string, string>> _values = new();
    private readonly Dictionary<string, string> _current = new();

    public void Add(string name, string version, string value, bool makeCurrent)
    {
        if (!_values.TryGetValue(name, out var versions))
            _values[name] = versions = new Dictionary<string, string>();

        versions[version] = value;
        if (makeCurrent) _current[name] = version;
    }

    public string CurrentVersion(string name) => _current[name];

    public string Resolve(string reference)
    {
        var parts = reference.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var name = parts[0];
        var version = parts.Length > 1 ? parts[1] : _current[name];
        return _values[name][version];
    }
}