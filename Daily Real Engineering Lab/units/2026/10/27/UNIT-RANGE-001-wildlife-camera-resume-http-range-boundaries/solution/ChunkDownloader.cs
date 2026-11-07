using System.Net.Http.Headers;

namespace RangeLab;

// Reference implementation: compare after attempting the exercise.
// Replace starter/RangeLab/ChunkDownloader.cs with this file to self-check.
public sealed class ChunkDownloader
{
    private readonly HttpClient _client;
    private readonly int _chunkSize;

    public ChunkDownloader(HttpClient client, int chunkSize)
    {
        _client = client;
        _chunkSize = chunkSize > 0 ? chunkSize : throw new ArgumentOutOfRangeException(nameof(chunkSize));
    }

    public async Task<byte[]> DownloadAsync(int totalLength, CancellationToken cancellationToken = default)
    {
        if (totalLength <= 0) throw new ArgumentOutOfRangeException(nameof(totalLength));

        using var destination = new MemoryStream();
        for (long start = 0; start < totalLength; start += _chunkSize)
        {
            var end = Math.Min(totalLength - 1L, start + _chunkSize - 1L);
            using var request = new HttpRequestMessage(HttpMethod.Get, "https://fixture.invalid/camera-image");
            request.Headers.Range = new RangeHeaderValue(start, end);

            using var response = await _client.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();
            var chunk = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var contentRange = response.Content.Headers.ContentRange;
            if (response.StatusCode != System.Net.HttpStatusCode.PartialContent ||
                contentRange?.From != start || contentRange.To != end ||
                contentRange.Length != totalLength || chunk.LongLength != end - start + 1)
            {
                throw new InvalidDataException("Server returned an inconsistent byte range.");
            }
            await destination.WriteAsync(chunk, cancellationToken);
        }

        return destination.ToArray();
    }
}
