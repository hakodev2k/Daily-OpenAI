using System.Diagnostics;
namespace TraceWindowLab;

public static class LabTrace
{
    public static readonly ActivitySource Source = new("TraceWindowLab.Courier", "1.0.0");
}

public sealed record DispatchResult(string SpecimenId, string Status);

public sealed class DispatchService
{
    private readonly ControlledGateway _gateway;
    public DispatchService(ControlledGateway gateway) => _gateway = gateway;

    public Task<DispatchResult> DispatchAsync(string specimenId)
    {
        using var activity = LabTrace.Source.StartActivity("dispatch");
        activity?.SetTag("specimen.id", specimenId);
        return _gateway.SendAsync(specimenId);
    }
}
