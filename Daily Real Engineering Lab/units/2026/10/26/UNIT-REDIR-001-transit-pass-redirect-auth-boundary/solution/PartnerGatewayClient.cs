using System.Net;
using System.Net.Http.Headers;

internal sealed class PartnerGatewayClient : IDisposable
{
    private const int MaxRedirects = 3;
    private readonly Uri _origin;
    private readonly HttpClient _client;

    public PartnerGatewayClient(Uri partnerOrigin)
    {
        _origin = partnerOrigin;
        _client = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseCookies = false
        })
        {
            BaseAddress = partnerOrigin,
            Timeout = TimeSpan.FromSeconds(5)
        };
    }

    public async Task<HttpStatusCode> CheckAsync(string route, CancellationToken cancellationToken = default)
    {
        var target = new Uri(_origin, route);
        for (var redirects = 0; redirects <= MaxRedirects; redirects++)
        {
            if (!SameOrigin(_origin, target))
                throw new InvalidOperationException("Untrusted redirect destination.");

            using var request = new HttpRequestMessage(HttpMethod.Get, target);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "demo-token");
            using var response = await _client.SendAsync(request, cancellationToken);

            if (!IsRedirect(response.StatusCode) || response.Headers.Location is null)
                return response.StatusCode;

            if (redirects == MaxRedirects)
                throw new InvalidOperationException("Redirect hop limit exceeded.");

            target = new Uri(target, response.Headers.Location);
        }

        throw new InvalidOperationException("Redirect hop limit exceeded.");
    }

    private static bool IsRedirect(HttpStatusCode status) =>
        status is HttpStatusCode.MovedPermanently or HttpStatusCode.Redirect
            or HttpStatusCode.TemporaryRedirect or HttpStatusCode.PermanentRedirect;

    private static bool SameOrigin(Uri left, Uri right) =>
        left.Scheme.Equals(right.Scheme, StringComparison.OrdinalIgnoreCase) &&
        left.IdnHost.Equals(right.IdnHost, StringComparison.OrdinalIgnoreCase) &&
        left.Port == right.Port &&
        string.IsNullOrEmpty(right.UserInfo);

    public void Dispose() => _client.Dispose();
}
