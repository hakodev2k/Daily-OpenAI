namespace CancellationLab;

public sealed record ScanRequest(string DocumentId, string Sha256);
public sealed record ScanReceipt(string DocumentId, string Verdict);

public interface IExternalScanner
{
    Task<ScanReceipt> ScanAsync(ScanRequest request, CancellationToken cancellationToken);
}

public sealed class DocumentValidationService
{
    private readonly IExternalScanner _scanner;

    public DocumentValidationService(IExternalScanner scanner) => _scanner = scanner;

    public async Task<ScanReceipt> ValidateAsync(
        ScanRequest request,
        CancellationToken requestAborted,
        CancellationToken applicationStopping)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            requestAborted, applicationStopping);

        return await _scanner.ScanAsync(request, linked.Token).ConfigureAwait(false);
    }
}
