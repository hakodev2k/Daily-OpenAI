using System.Net.Http;

namespace CanalGate.Export;

public sealed class SensorExportClient
{
    private readonly HttpClient _http;

    public SensorExportClient(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    // Contract: caller owns the returned stream and must dispose it.
    // Export files can be large; avoid buffering the entire response.
    public async Task<Stream> OpenExportAsync(
        string gateId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(gateId))
            throw new ArgumentException("Gate ID is required.", nameof(gateId));

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"exports/{Uri.EscapeDataString(gateId)}");

        using var response = await _http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}
