using System.Net;
using System.Net.Http.Headers;

namespace RareBookLab;

public sealed class CatalogClient
{
    private readonly HttpClient _http;
    private string? _etag;
    private string? _cachedJson;

    public CatalogClient(HttpClient http) => _http = http;

    // The memory budget is managed independently from HTTP request metadata.
    public void EvictBodyForMemoryPressure() => _cachedJson = null;

    public async Task<string> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get, "https://catalog.example.invalid/api/rare-books");

        if (_etag is not null)
            request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(_etag));

        using var response = await _http.SendAsync(
            request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotModified)
            return _cachedJson ?? string.Empty;

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        _etag = response.Headers.ETag?.Tag;
        _cachedJson = json;
        return json;
    }
}
