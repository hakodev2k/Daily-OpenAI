using System.Net;
using System.Net.Http.Headers;

namespace RareBookLab;

public sealed class CatalogClient
{
    private readonly HttpClient _http;
    private string? _etag;
    private string? _cachedJson;

    public CatalogClient(HttpClient http) => _http = http;

    public void EvictBodyForMemoryPressure() => _cachedJson = null;

    public async Task<string> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get, "https://catalog.example.invalid/api/rare-books");

        if (_cachedJson is not null && _etag is not null)
            request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(_etag));

        using var response = await _http.SendAsync(
            request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotModified)
            return _cachedJson ?? throw new InvalidOperationException(
                "Received HTTP 304 without a locally available representation.");

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        _etag = response.Headers.ETag?.Tag;
        _cachedJson = json;
        return json;
    }
}
