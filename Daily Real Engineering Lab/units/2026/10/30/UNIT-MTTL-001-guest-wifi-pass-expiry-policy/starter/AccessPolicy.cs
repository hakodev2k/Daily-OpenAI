namespace VisitorPassLab;

public static class AccessPolicy
{
    public static bool CanConnect(
        PassStore store,
        string passId,
        DateTimeOffset nowUtc)
    {
        var document = store.Find(passId);
        return document is not null;
    }
}
