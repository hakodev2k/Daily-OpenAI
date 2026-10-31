namespace PatrolLab;

// Reference replacement for starter/NotificationBatchScheduler.cs.
public sealed class NotificationBatchScheduler
{
    public void Schedule(
        IReadOnlyList<PatrolNotice> notices,
        DeferredQueue queue,
        FakeGateway gateway)
    {
        foreach (var notice in notices)
        {
            PatrolNotice command = notice;
            queue.Enqueue(() => gateway.Send(command));
        }
    }
}
