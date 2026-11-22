using System.Net;
using System.Net.Http;
using System.Text.Json;

var gateway = new TenantGateway(new HttpClient(new RecordingHandler()));
var a = gateway.Prepare("tenant-alpha", "invoice-100");
var b = gateway.Prepare("tenant-beta", "invoice-200");
var responses = await Task.WhenAll(a(), b());
foreach (var response in responses) Console.WriteLine(response);
if (responses[0] != "invoice-100:tenant-alpha" || responses[1] != "invoice-200:tenant-beta")
    throw new InvalidOperationException("Tenant isolation contract failed");

sealed class TenantGateway(HttpClient client)
{
    public Func<Task<string>> Prepare(string tenant, string invoice)
    {
        client.DefaultRequestHeaders.Remove("X-Tenant-Id");
        client.DefaultRequestHeaders.Add("X-Tenant-Id", tenant);
        return async () =>
        {
            using var response = await client.GetAsync("https://billing.invalid/invoices/" + invoice);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        };
    }
}

sealed class RecordingHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var tenant = request.Headers.GetValues("X-Tenant-Id").Single();
        var invoice = request.RequestUri!.Segments.Last();
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(invoice + ":" + tenant)
        });
    }
}
