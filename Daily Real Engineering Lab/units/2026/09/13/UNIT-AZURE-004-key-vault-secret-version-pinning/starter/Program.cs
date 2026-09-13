var vault = new FakeSecretVault();
var paymentProvider = new FakePaymentProvider(acceptedCredential: "pay-key-v2");

// Simulates application configuration that was deployed before the security team rotated the secret.
var configuredSecretReference = "https://fake.vault/secrets/PaymentApiKey/v1";

Console.WriteLine($"Configured secret reference: {configuredSecretReference}");
Console.WriteLine($"Vault current version: {vault.GetCurrentVersion("PaymentApiKey")}");

var resolved = vault.Resolve(configuredSecretReference);
Console.WriteLine($"Application resolved version: {resolved.Version}");

var statusCode = paymentProvider.SendRequest(resolved.Value);
Console.WriteLine($"Payment provider status: {statusCode}");

if (statusCode != 200)
{
    Console.WriteLine("Request failed after credential rotation.");
    return 1;
}

Console.WriteLine("Request accepted.");
return 0;

internal sealed class FakeSecretVault
{
    private readonly Dictionary<string, SortedDictionary<int, string>> _secrets = new(StringComparer.Ordinal)
    {
        ["PaymentApiKey"] = new SortedDictionary<int, string>
        {
            [1] = "pay-key-v1",
            [2] = "pay-key-v2"
        }
    };

    public string GetCurrentVersion(string name)
    {
        var versions = _secrets[name];
        return $"v{versions.Keys.Max()}";
    }

    public ResolvedSecret Resolve(string reference)
    {
        var uri = new Uri(reference);
        var segments = uri.AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length is < 2 or > 3 || !segments[0].Equals("secrets", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Unsupported secret reference: {reference}");
        }

        var name = segments[1];
        var versions = _secrets[name];

        var versionNumber = segments.Length == 3
            ? ParseVersion(segments[2])
            : versions.Keys.Max();

        return new ResolvedSecret(name, $"v{versionNumber}", versions[versionNumber]);
    }

    private static int ParseVersion(string value)
    {
        if (value.Length < 2 || value[0] != 'v' || !int.TryParse(value[1..], out var version))
        {
            throw new InvalidOperationException($"Invalid version segment: {value}");
        }

        return version;
    }
}

internal sealed class FakePaymentProvider(string acceptedCredential)
{
    public int SendRequest(string credential)
        => string.Equals(credential, acceptedCredential, StringComparison.Ordinal) ? 200 : 401;
}

internal sealed record ResolvedSecret(string Name, string Version, string Value);
