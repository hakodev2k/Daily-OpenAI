using System.Text.Json;

namespace BulkIngestionLab;

public sealed class BulkIngestor
{
    private const int MaxAttempts = 3;

    public async Task<IngestionReport> IngestAsync(
        IReadOnlyList<SearchDocument> documents,
        FakeSearchGateway gateway,
        CancellationToken cancellationToken)
    {
        var indexed = new List<string>();
        var quarantined = new List<string>();
        var pending = documents.ToList();

        for (var attempt = 1; attempt <= MaxAttempts && pending.Count > 0; attempt++)
        {
            using var response = await gateway.SendBulkAsync(pending, cancellationToken);
            response.EnsureSuccessStatusCode();
            using var payload = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync(cancellationToken));

            var items = payload.RootElement.GetProperty("items").EnumerateArray().ToArray();
            if (items.Length != pending.Count)
                throw new InvalidDataException("Bulk item count does not match submitted batch.");

            var retry = new List<SearchDocument>();
            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i].GetProperty("index");
                var id = item.GetProperty("_id").GetString();
                if (!StringComparer.Ordinal.Equals(id, pending[i].Id))
                    throw new InvalidDataException("Bulk response ID/order contract mismatch.");

                var status = item.GetProperty("status").GetInt32();
                if (status is >= 200 and < 300)
                {
                    indexed.Add(pending[i].Id);
                }
                else if (status == 429 || status is >= 500 and <= 599)
                {
                    retry.Add(pending[i]);
                }
                else
                {
                    quarantined.Add(pending[i].Id);
                }
            }
            pending = retry;
        }

        return new IngestionReport(
            indexed,
            quarantined,
            pending.Select(x => x.Id).ToArray());
    }
}
