using System.Net.Http;
using System.Text;

namespace OutboundTenantLab;

public sealed class PartnerClient
{
    private readonly HttpClient _http;

    public PartnerClient(HttpClient http) => _http = http;

    // Investigation: trạng thái nào cần thuộc riêng từng lần gửi?
    public async Task SubmitAsync(string tenantId, string orderId, Func<Task> beforeSend)
    {
        _http.DefaultRequestHeaders.Remove("X-Tenant-Id");
        _http.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId);

        await beforeSend();

        using var content = new StringContent("{}", Encoding.UTF8, "application/json");
        using var response = await _http.PostAsync($"orders/{orderId}", content);
        response.EnsureSuccessStatusCode();
    }
}
