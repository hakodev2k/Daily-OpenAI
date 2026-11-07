namespace TraceWindowLab;

public sealed class ControlledGateway
{
    private readonly TaskCompletionSource<bool> _start =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _entered =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _finish =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task Entered => _entered.Task;
    public void AllowStart() => _start.TrySetResult(true);
    public void AllowFinish() => _finish.TrySetResult(true);

    public async Task<DispatchResult> SendAsync(string specimenId)
    {
        await _start.Task.ConfigureAwait(false);
        using var activity = LabTrace.Source.StartActivity("gateway-send");
        activity?.SetTag("specimen.id", specimenId);
        _entered.TrySetResult(true);
        await _finish.Task.ConfigureAwait(false);
        return new DispatchResult(specimenId, "accepted");
    }
}
