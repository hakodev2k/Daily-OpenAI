using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading;

public sealed record QuoteProbe(HttpStatusCode StatusCode, string ObservedSession);

// Reference only: copy into starter/TenantGateway.cs AFTER your own investigation.
public sealed class TenantGateway : IDisposable
{
    private readonly Uri _baseAddress;
    private readonly ConcurrentDictionary<string, Lazy<HttpClient>> _tenantClients =
        new(StringComparer.Ordinal);

    public TenantGateway(Uri partnerBaseAddress) => _baseAddress = partnerBaseAddress;

    private HttpClient ClientFor(string tenant) =>
        _tenantClients.GetOrAdd(tenant, _ =>
            new Lazy<HttpClient>(() =>
                new HttpClient(new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = new CookieContainer()
                })
                {
                    BaseAddress = _baseAddress
                }, LazyThreadSafetyMode.ExecutionAndPublication)).Value;

    public async Task AuthenticateAsync(string tenant)
    {
        using var response = await ClientFor(tenant)
            .PostAsync($"auth/{Uri.EscapeDataString(tenant)}", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task<QuoteProbe> GetQuoteAsync(string tenant)
    {
        using var response = await ClientFor(tenant)
            .GetAsync($"quote/{Uri.EscapeDataString(tenant)}");
        var observed = response.Headers.TryGetValues("X-Lab-Observed-Session", out var values)
            ? values.Single()
            : "missing";
        return new QuoteProbe(response.StatusCode, observed);
    }

    public void Dispose()
    {
        foreach (var lazy in _tenantClients.Values)
            if (lazy.IsValueCreated) lazy.Value.Dispose();
    }
}
