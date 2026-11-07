using System.Net;
using System.Net.Http.Json;

public sealed class LabelGateway
{
    private readonly HttpClient _http;
    public LabelGateway(HttpClient http) => _http = http;

    public async Task<HttpStatusCode> SubmitAsync(string shipmentId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("labels/submit", new { shipmentId }, cancellationToken);
        return response.StatusCode;
    }

    public async Task<HttpStatusCode> GetStatusAsync(string shipmentId, CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync("labels/status/" + Uri.EscapeDataString(shipmentId), cancellationToken);
        return response.StatusCode;
    }
}
