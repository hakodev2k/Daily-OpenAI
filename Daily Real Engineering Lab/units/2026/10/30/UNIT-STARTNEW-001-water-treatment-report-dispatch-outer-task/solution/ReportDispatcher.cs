namespace WaterReports;

public interface IReportGateway
{
    Task SendAsync(CancellationToken cancellationToken);
}

public sealed class ReportDispatcher
{
    private readonly IReportGateway _gateway;

    public ReportDispatcher(IReportGateway gateway) => _gateway = gateway;

    public Task DispatchAsync(CancellationToken cancellationToken = default) =>
        _gateway.SendAsync(cancellationToken);
}
