using System.Net;
public sealed class PartnerClient
{
    public async Task<string> ProbeAsync(Uri partnerBaseUri, CancellationToken cancellationToken)
    {
        using var handler = new SocketsHttpHandler { MaxConnectionsPerServer = 1, UseProxy = false };
        using var http = new HttpClient(handler, disposeHandler: false) { Timeout = Timeout.InfiniteTimeSpan };
        var report = await http.GetAsync(
            new Uri(partnerBaseUri, "report"),
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        if (report.StatusCode != HttpStatusCode.OK ||
            !report.Headers.TryGetValues("X-Export-State", out var states) ||
            !states.Contains("ready", StringComparer.Ordinal))
        {
            throw new InvalidOperationException("Partner report is not ready.");
        }
        using var healthDeadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        healthDeadline.CancelAfter(TimeSpan.FromMilliseconds(900));
        using var health = await http.GetAsync(
            new Uri(partnerBaseUri, "health"),
            HttpCompletionOption.ResponseHeadersRead,
            healthDeadline.Token);
        health.EnsureSuccessStatusCode();
        return await health.Content.ReadAsStringAsync(healthDeadline.Token);
    }
}
