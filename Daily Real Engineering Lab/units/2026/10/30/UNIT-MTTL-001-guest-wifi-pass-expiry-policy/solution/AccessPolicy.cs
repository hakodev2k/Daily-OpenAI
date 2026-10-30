namespace VisitorPassLab;

public static class AccessPolicy
{
    public static bool CanConnect(
        PassStore store,
        string passId,
        DateTimeOffset nowUtc)
    {
        var pass = store.Find(passId);
        if (pass is null) return false;
        if (nowUtc >= pass.ExpiresAtUtc) return false;
        if (pass.RevokedAtUtc is { } revokedAtUtc && nowUtc >= revokedAtUtc)
            return false;
        return true;
    }
}
