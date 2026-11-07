using System.Net.Http.Json;

public sealed class PartnerGateway : IDisposable
{
    private readonly HttpClient _client;

    public PartnerGateway(Uri partnerOrigin)
    {
        _client = new HttpClient
        {
            BaseAddress = partnerOrigin,
            Timeout = TimeSpan.FromSeconds(5)
        };
    }

    public async Task<SubmissionResult> SubmitAsync(string entryPath, Evidence evidence, CancellationToken ct)
    {
        try
        {
            using var response = await _client.PostAsJsonAsync(entryPath, evidence, ct);
            return new SubmissionResult(response.IsSuccessStatusCode, null, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return new SubmissionResult(false, null, 0);
        }
    }

    public void Dispose() => _client.Dispose();
}
