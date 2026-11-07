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
        await foreach (var row in JsonSerializer.DeserializeAsyncEnumerable<IntakeRow>(
            input, cancellationToken: cancellationToken))
        {
            if (row is null)
                throw new InvalidDataException("Null row is not allowed.");

            await store.UpsertAsync(row, cancellationToken);
        }
    }
}
