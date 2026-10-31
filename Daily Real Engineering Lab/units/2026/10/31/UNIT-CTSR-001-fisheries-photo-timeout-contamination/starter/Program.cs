using CoastalInspection;

var mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
var service = new PhotoInspectionService(new SimulatedPartnerVerifier());
var first = await service.InspectAsync("PHOTO-001");
var second = await service.InspectAsync("PHOTO-002");
var third = await service.InspectAsync("PHOTO-003");

using var cancelledRequest = new CancellationTokenSource();
cancelledRequest.Cancel();
var callerCancellationPropagated = false;
try { await service.InspectAsync("PHOTO-004", cancelledRequest.Token); }
catch (OperationCanceledException) { callerCancellationPropagated = true; }

Console.WriteLine($"first={first}; second={second}; third={third}; callerCancelled={callerCancellationPropagated}");

var baseline = first == InspectionResult.TimedOut
    && second == InspectionResult.TimedOut
    && third == InspectionResult.TimedOut
    && callerCancellationPropagated;
var fixedContract = first == InspectionResult.TimedOut
    && second == InspectionResult.Accepted
    && third == InspectionResult.Accepted
    && callerCancellationPropagated;

switch (mode)
{
    case "demo": return 0;
    case "reproduce":
        Console.WriteLine(baseline ? "REPRODUCE PASS" : "REPRODUCE FAIL");
        return baseline ? 0 : 1;
    case "verify":
        Console.WriteLine(fixedContract ? "VERIFY PASS" : "VERIFY FAIL");
        return fixedContract ? 0 : 2;
    default:
        Console.Error.WriteLine("Usage: demo | reproduce | verify");
        return 64;
}

internal sealed class SimulatedPartnerVerifier : IPhotoVerifier
{
    public Task CheckAsync(string photoId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return photoId == "PHOTO-001"
            ? Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken)
            : Task.CompletedTask;
    }
}
