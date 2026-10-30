using System.Net;
using System.Net.Http;

namespace PartnerCookieLab;

public sealed record PartnerProfileResult(string RequestedTenant, int StatusCode, string Body);

public sealed class PartnerGateway : IDisposable
{
    private readonly Uri _partnerBaseAddress;
    private readonly object _gate = new();
    private readonly Dictionary<string, HttpClient> _clients = new(StringComparer.Ordinal);

    public PartnerGateway(Uri partnerBaseAddress) => _partnerBaseAddress = partnerBaseAddress;

    private HttpClient ClientFor(string tenant)
    {
        lock (_gate)
        {
            if (_clients.TryGetValue(tenant, out var existing))
                return existing;

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler) { BaseAddress = _partnerBaseAddress };
            _clients.Add(tenant, client);
            return client;
        }
    }

    public async Task<int> StartSessionAsync(string tenant, CancellationToken cancellationToken = default)
    {
        using var response = await ClientFor(tenant).PostAsync(
            $"partner/session/{Uri.EscapeDataString(tenant)}", null, cancellationToken);
        return (int)response.StatusCode;
    }

    public async Task<PartnerProfileResult> ReadProfileAsync(
        string tenant, CancellationToken cancellationToken = default)
    {
        using var response = await ClientFor(tenant).GetAsync("partner/profile", cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return new PartnerProfileResult(tenant, (int)response.StatusCode, body);
    }

    public void Dispose()
    {
        lock (_gate)
        {
            foreach (var client in _clients.Values)
                client.Dispose();
            _clients.Clear();
        }
    }
}
