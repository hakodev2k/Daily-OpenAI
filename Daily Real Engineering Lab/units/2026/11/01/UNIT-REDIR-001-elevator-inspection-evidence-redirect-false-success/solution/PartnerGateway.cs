using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

public sealed class PartnerGateway : IDisposable
{
    private readonly Uri _origin;
    private readonly HttpClient _client;

    public PartnerGateway(Uri partnerOrigin)
    {
        _origin = partnerOrigin;
        _client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false })
        {
            BaseAddress = partnerOrigin,
            Timeout = TimeSpan.FromSeconds(5)
        };
    }

    public async Task<SubmissionResult> SubmitAsync(string entryPath, Evidence evidence, CancellationToken ct)
    {
        try
        {
            using var first = await _client.PostAsJsonAsync(entryPath, evidence, ct);
            if (first.StatusCode != HttpStatusCode.Redirect)
                return await ParseReceiptAsync(first, evidence.Id, ct);

            var location = first.Headers.Location;
            if (location is null)
                return new SubmissionResult(false, null, (int)first.StatusCode);

            var target = new Uri(_origin, location);
            if (!IsApprovedIngestTarget(target))
                return new SubmissionResult(false, null, (int)first.StatusCode);

            // Only this explicitly approved target may receive a new POST.
            using var second = await _client.PostAsJsonAsync(target, evidence, ct);
            return await ParseReceiptAsync(second, evidence.Id, ct);
        }
        catch (HttpRequestException)
        {
            return new SubmissionResult(false, null, 0);
        }
    }

    private bool IsApprovedIngestTarget(Uri uri) =>
        uri.Scheme == _origin.Scheme
        && uri.Host.Equals(_origin.Host, StringComparison.OrdinalIgnoreCase)
        && uri.Port == _origin.Port
        && uri.UserInfo.Length == 0
        && uri.AbsolutePath == "/api/evidence"
        && uri.Query.Length == 0
        && uri.Fragment.Length == 0;

    private static async Task<SubmissionResult> ParseReceiptAsync(
        HttpResponseMessage response, string evidenceId, CancellationToken ct)
    {
        var status = (int)response.StatusCode;
        if (response.StatusCode != HttpStatusCode.OK
            || response.Content.Headers.ContentType?.MediaType != "application/json")
            return new SubmissionResult(false, null, status);

        try
        {
            var receipt = await response.Content.ReadFromJsonAsync<Receipt>(cancellationToken: ct);
            return receipt is { Accepted: true } && receipt.Id == "RCPT-" + evidenceId
                ? new SubmissionResult(true, receipt.Id, status)
                : new SubmissionResult(false, null, status);
        }
        catch (JsonException)
        {
            return new SubmissionResult(false, null, status);
        }
    }

    public void Dispose() => _client.Dispose();
}
