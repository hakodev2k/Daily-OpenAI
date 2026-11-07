namespace CoastalInspection;

public enum InspectionResult { Accepted, TimedOut }

public interface IPhotoVerifier
{
    Task CheckAsync(string photoId, CancellationToken cancellationToken);
}

public sealed class PhotoInspectionService
{
    private static readonly TimeSpan PartnerDeadline = TimeSpan.FromMilliseconds(250);
    private readonly IPhotoVerifier _verifier;

    public PhotoInspectionService(IPhotoVerifier verifier) => _verifier = verifier;

    public async Task<InspectionResult> InspectAsync(
        string photoId, CancellationToken requestCancellation = default)
    {
        using var operation = CancellationTokenSource.CreateLinkedTokenSource(requestCancellation);
        operation.CancelAfter(PartnerDeadline);

        try
        {
            await _verifier.CheckAsync(photoId, operation.Token);
            return InspectionResult.Accepted;
        }
        catch (OperationCanceledException)
            when (!requestCancellation.IsCancellationRequested && operation.IsCancellationRequested)
        {
            return InspectionResult.TimedOut;
        }
    }
}
