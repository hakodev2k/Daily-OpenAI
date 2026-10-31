namespace PatrolLab;

// Immutable reproduction fixture. Do not edit during the lab.
public sealed class BaselineScheduler
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
