namespace NotificationLab;

public sealed record DeliveryReceipt(string UserId, string Message, bool Sent);

public interface ITemplateRenderer
{
    string Render(string userId);
}

public interface IMessageGateway
{
    void Send(string userId, string message);
}

public interface IAuditWriter
{
    void Record(string userId);
}

public sealed class NotificationCoordinator(
    ITemplateRenderer renderer,
    IMessageGateway gateway,
    IAuditWriter audit)
{
    public DeliveryReceipt Deliver(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        // This order changed during an internal refactor. The observable contract
        // of this method did not intentionally change.
        audit.Record(userId);
        var message = renderer.Render(userId);
        gateway.Send(userId, message);

        return new DeliveryReceipt(userId, message, Sent: true);
    }
}

public sealed class CollaborationProbe : ITemplateRenderer, IMessageGateway, IAuditWriter
{
    public List<string> Calls { get; } = [];
    public List<(string UserId, string Message)> SentMessages { get; } = [];
    public List<string> AuditUsers { get; } = [];

    public string Render(string userId)
    {
        Calls.Add("template");
        return $"Hello {userId}";
    }

    public void Send(string userId, string message)
    {
        Calls.Add("send");
        SentMessages.Add((userId, message));
    }

    public void Record(string userId)
    {
        Calls.Add("audit");
        AuditUsers.Add(userId);
    }
}
