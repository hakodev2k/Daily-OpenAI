namespace Lab;

public sealed record DispatchMessage(
    string Id,
    string TenantId,
    string ChargerId,
    bool IsPoison);

public static class SessionPartitioner
{
    // SessionId được tính tại ingress, trước khi message vào broker.
    public static string ResolveSessionId(DispatchMessage message)
    {
        return message.TenantId;
    }
}
