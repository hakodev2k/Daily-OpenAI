using System.Net;
using System.Net.Http;

namespace PartnerCookieLab;

public sealed record PartnerProfileResult(string RequestedTenant, int StatusCode, string Body);

public sealed class PartnerGateway : IDisposable
{
    private readonly HttpClient _http;

    public PartnerGateway(Uri partnerBaseAddress)
    {
        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer(),
            AllowAutoRedirect = false
        };
        _http = new HttpClient(handler) { BaseAddress = partnerBaseAddress };
    }

    public async Task<int> StartSessionAsync(string tenant, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsync(
            $"partner/session/{Uri.EscapeDataString(tenant)}", null, cancellationToken);
        return (int)response.StatusCode;
    }

    public async Task<PartnerProfileResult> ReadProfileAsync(
        string tenant, CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync("partner/profile", cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return new PartnerProfileResult(tenant, (int)response.StatusCode, body);
    }

    public void Dispose() => _http.Dispose();
}
