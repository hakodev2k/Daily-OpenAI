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
    private readonly CancellationTokenSource _deadline = new();

    public PhotoInspectionService(IPhotoVerifier verifier) => _verifier = verifier;

    public async Task<InspectionResult> InspectAsync(
        string photoId, CancellationToken requestCancellation = default)
    {
        _deadline.CancelAfter(PartnerDeadline);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            requestCancellation, _deadline.Token);

        try
        {
            await _verifier.CheckAsync(photoId, linked.Token);
            return InspectionResult.Accepted;
        }
        catch (OperationCanceledException)
            when (!requestCancellation.IsCancellationRequested && _deadline.IsCancellationRequested)
        {
            return InspectionResult.TimedOut;
        }
    }
}
