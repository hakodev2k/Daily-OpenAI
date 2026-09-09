namespace SharedStateLab;

public sealed record RenewalOptions(string Provider);

public sealed class RenewalRouter
{
    private readonly RenewalOptions _options;

    public RenewalRouter(RenewalOptions options)
    {
        _options = options;
    }

    public string SelectProvider(string subscriptionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);
        return _options.Provider;
    }
}
