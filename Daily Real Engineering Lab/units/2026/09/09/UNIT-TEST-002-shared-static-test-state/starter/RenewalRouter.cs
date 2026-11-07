namespace SharedStateLab;

public static class RenewalProviderSettings
{
    public static string Provider { get; set; } = "Legacy";
}

public sealed class RenewalRouter
{
    public string SelectProvider(string subscriptionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);
        return RenewalProviderSettings.Provider;
    }
}
