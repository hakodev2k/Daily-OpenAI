namespace BulkIngestionLab;

public sealed class BulkIngestor
{
    public async Task<IngestionReport> IngestAsync(
        IReadOnlyList<SearchDocument> documents,
        FakeSearchGateway gateway,
        CancellationToken cancellationToken)
    {
        // Existing integration adapter: request-level status is recorded for operations.
        using var response = await gateway.SendBulkAsync(documents, cancellationToken);
        response.EnsureSuccessStatusCode();
        return new IngestionReport(
            documents.Select(d => d.Id).ToArray(),
            Array.Empty<string>(),
            Array.Empty<string>());
    }
}
