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
        for (var attempt = 1; attempt <= 2; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var request = new HttpRequestMessage(
                HttpMethod.Put, $"holds/{Uri.EscapeDataString(holdId)}");
            request.Headers.TryAddWithoutValidation("Idempotency-Key", holdId);
            request.Content = new StringContent(
                JsonSerializer.Serialize(new { holdId }), Encoding.UTF8, "application/json");

            using var response = await _client.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable && attempt < 2)
                continue;

            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }

        throw new InvalidOperationException("Retry policy exhausted unexpectedly.");
    }
}
