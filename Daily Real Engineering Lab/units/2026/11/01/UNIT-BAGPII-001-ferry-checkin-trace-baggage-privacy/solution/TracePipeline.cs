using System.Diagnostics;

namespace FerryCheckIn;

public sealed record CheckInRequest(string RequestId, string PassengerEmail, string PassportLast4, string Gate);
public sealed record TraceExport(string TraceId, string BaggageHeader, string LogLine, string BusinessResult);

public static class CheckInPipeline
{
    private static readonly ActivitySource Source = new("Ferry.CheckIn");

    public static TraceExport Execute(CheckInRequest request)
    {
        using var activity = Source.StartActivity("checkin.accept", ActivityKind.Server)
            ?? throw new InvalidOperationException("An ActivityListener is required for this local lab.");

        // An opaque correlation identifier is enough for the cross-service contract.
        activity.AddBaggage("request_id", request.RequestId);
        activity.SetTag("gate", request.Gate);
        activity.SetTag("checkin.result", "accepted");

        // Simplified local stand-in for an outbound baggage header; not a production propagator.
        var outboundHeader = string.Join(",",
            activity.Baggage.Select(pair =>
                $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value ?? "")}"));

        var log = $"checkin.accepted trace={activity.TraceId} gate={request.Gate}";
        return new TraceExport(activity.TraceId.ToString(), outboundHeader, log, "ACCEPTED");
    }
}
