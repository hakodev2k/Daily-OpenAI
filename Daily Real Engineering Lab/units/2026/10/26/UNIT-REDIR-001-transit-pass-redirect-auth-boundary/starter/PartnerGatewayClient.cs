using System.Net;
using System.Net.Http.Headers;

internal sealed class PartnerGatewayClient : IDisposable
{
    private readonly HttpClient _client;

    public PartnerGatewayClient(Uri partnerOrigin)
    {
        _client = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = true,
            UseCookies = false
        })
        {
            BaseAddress = partnerOrigin,
            Timeout = TimeSpan.FromSeconds(5)
        };
    }

    public async Task<HttpStatusCode> CheckAsync(string route, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, route);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "demo-token");
        using var response = await _client.SendAsync(request, cancellationToken);
        return response.StatusCode;
    }

    public void Dispose() => _client.Dispose();
}
