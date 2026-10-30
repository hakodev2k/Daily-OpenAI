namespace WaterReports;

public interface IReportGateway
{
    Task SendAsync(CancellationToken cancellationToken);
}

public sealed class ReportDispatcher
{
    private readonly IReportGateway _gateway;

    public ReportDispatcher(IReportGateway gateway) => _gateway = gateway;

    public async Task DispatchAsync(CancellationToken cancellationToken = default)
    {
        await Task.Factory.StartNew(async () =>
        {
            await _gateway.SendAsync(cancellationToken);
        }, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
    }
}
