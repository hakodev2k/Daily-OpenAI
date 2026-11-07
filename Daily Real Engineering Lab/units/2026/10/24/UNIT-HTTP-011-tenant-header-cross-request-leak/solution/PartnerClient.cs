using System.Net.Http;
using System.Text;

namespace OutboundTenantLab;

public sealed class PartnerClient
{
    private readonly HttpClient _http;

    public PartnerClient(HttpClient http) => _http = http;

    public async Task SubmitAsync(string tenantId, string orderId, Func<Task> beforeSend)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"orders/{orderId}");
        request.Headers.Add("X-Tenant-Id", tenantId);
        request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        await beforeSend();

        using var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
