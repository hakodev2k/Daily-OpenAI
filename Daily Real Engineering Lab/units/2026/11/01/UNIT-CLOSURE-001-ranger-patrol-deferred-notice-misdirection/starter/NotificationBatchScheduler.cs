namespace PatrolLab;

// Learner-editable production component. Preserve deferred dispatch.
public sealed class NotificationBatchScheduler
{
    public void Schedule(
        IReadOnlyList<PatrolNotice> notices,
        DeferredQueue queue,
        FakeGateway gateway)
    {
        PatrolNotice? selected = null;

        foreach (var notice in notices)
        {
            selected = notice;
            queue.Enqueue(() => gateway.Send(selected!));
        }
    }
}
