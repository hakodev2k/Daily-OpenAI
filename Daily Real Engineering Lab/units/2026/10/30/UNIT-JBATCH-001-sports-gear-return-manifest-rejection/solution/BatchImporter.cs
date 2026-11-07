using System.Text.Json;

namespace IntakeLab;

public sealed record IntakeRow(string Id, int Qty);

public sealed class BatchImporter
{
    public async Task ImportAsync(
        Stream input,
        ManifestStore store,
        CancellationToken cancellationToken = default)
    {
        var staged = new List<IntakeRow>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        await foreach (var row in JsonSerializer.DeserializeAsyncEnumerable<IntakeRow>(
            input, cancellationToken: cancellationToken))
        {
            if (row is null ||
                string.IsNullOrWhiteSpace(row.Id) ||
                row.Qty <= 0 ||
                !seen.Add(row.Id))
            {
                throw new InvalidDataException("Manifest row violates the batch contract.");
            }

            staged.Add(row);
        }

        await store.ApplyBatchAsync(staged, cancellationToken);
    }
}
