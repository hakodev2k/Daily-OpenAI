using System.Net;
using System.Text;
using System.Text.Json;

namespace PartnerRetryLab;

public sealed class PartnerHoldClient
{
    private readonly HttpClient _client;

    public PartnerHoldClient(HttpClient client) => _client = client;

    public async Task<HttpStatusCode> SubmitAsync(string holdId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Put, $"holds/{Uri.EscapeDataString(holdId)}");

        request.Headers.TryAddWithoutValidation("Idempotency-Key", holdId);
        request.Content = new StringContent(
            JsonSerializer.Serialize(new { holdId }), Encoding.UTF8, "application/json");

        for (var attempt = 1; attempt <= 2; attempt++)
        {
            using var response = await _client.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable && attempt < 2)
                continue;

            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        throw new InvalidOperationException("Partner request was not accepted.");
    }
}
