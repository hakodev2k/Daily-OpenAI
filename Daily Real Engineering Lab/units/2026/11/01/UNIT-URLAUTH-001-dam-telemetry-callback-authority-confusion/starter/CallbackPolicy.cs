namespace CallbackGate;
public static class CallbackPolicy
{
    private const string ApprovedPrefix = "https://hooks.damtelemetry.example";
    public static bool IsAllowed(string callbackUrl) =>
        !string.IsNullOrWhiteSpace(callbackUrl)
        && callbackUrl.StartsWith(ApprovedPrefix, StringComparison.OrdinalIgnoreCase);
}
