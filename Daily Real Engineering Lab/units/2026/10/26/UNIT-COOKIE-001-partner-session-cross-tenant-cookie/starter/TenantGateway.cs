using System.Net;
using System.Net.Http;

public sealed record QuoteProbe(HttpStatusCode StatusCode, string ObservedSession);

// Adapter được chia sẻ bởi nhiều tenant trong cùng worker process.
public sealed class TenantGateway : IDisposable
{
    private readonly HttpClient _client;

    public TenantGateway(Uri partnerBaseAddress)
    {
        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };
        _client = new HttpClient(handler) { BaseAddress = partnerBaseAddress };
    }

    public async Task AuthenticateAsync(string tenant)
    {
        using var response = await _client.PostAsync($"auth/{Uri.EscapeDataString(tenant)}", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task<QuoteProbe> GetQuoteAsync(string tenant)
    {
        using var response = await _client.GetAsync($"quote/{Uri.EscapeDataString(tenant)}");
        var observed = response.Headers.TryGetValues("X-Lab-Observed-Session", out var values)
            ? values.Single()
            : "missing";
        return new QuoteProbe(response.StatusCode, observed);
    }

    public void Dispose() => _client.Dispose();
}
