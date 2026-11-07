namespace CallbackGate;
public static class CallbackPolicy
{
    private const string TrustedHost = "hooks.damtelemetry.example";
    public static bool IsAllowed(string callbackUrl)
    {
        if (!Uri.TryCreate(callbackUrl, UriKind.Absolute, out Uri? uri))
            return false;
        if (!string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            return false;
        if (!string.Equals(uri.IdnHost, TrustedHost, StringComparison.OrdinalIgnoreCase))
            return false;
        if (uri.Port != 443 || uri.UserInfo.Length != 0 || uri.Fragment.Length != 0)
            return false;
        return uri.AbsolutePath is "/ingest" or "/ingest/v2";
    }
}
