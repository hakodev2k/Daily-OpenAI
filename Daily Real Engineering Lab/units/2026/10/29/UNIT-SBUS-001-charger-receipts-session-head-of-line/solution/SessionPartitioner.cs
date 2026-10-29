namespace Lab;

public sealed record DispatchMessage(
    string Id,
    string TenantId,
    string ChargerId,
    bool IsPoison);

public static class SessionPartitioner
{
    public static string ResolveSessionId(DispatchMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (string.IsNullOrWhiteSpace(message.TenantId) || string.IsNullOrWhiteSpace(message.ChargerId))
            throw new ArgumentException("TenantId and ChargerId are required.");
        // Length-prefix prevents collisions if identifiers contain separators.
        return $"{message.TenantId.Length}:{message.TenantId}{message.ChargerId.Length}:{message.ChargerId}";
    }
}
