using System.Net;
using System.Net.Http.Headers;

var mode = args.FirstOrDefault() ?? "inspect";
using var partner = new FakePartner();
using var client = new HttpClient(partner) { BaseAddress = new Uri("https://partner.invalid/") };
var aConfigured = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var bConfigured = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

async Task DispatchAsync(string tenant, string token)
{
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    if (tenant == "A")
    {
        aConfigured.TrySetResult();
        await bConfigured.Task;
    }
    else
    {
        await aConfigured.Task;
        bConfigured.TrySetResult();
    }
    using var request = new HttpRequestMessage(HttpMethod.Post, "print");
    request.Headers.Add("X-Tenant", tenant);
    using var response = await client.SendAsync(request);
    response.EnsureSuccessStatusCode();
}
await Task.WhenAll(DispatchAsync("A", "school-a-secret"), DispatchAsync("B", "school-b-secret"));
var records = partner.Records.OrderBy(x => x.Tenant).ToArray();
foreach (var item in records)
    Console.WriteLine($"tenant={item.Tenant} expected={Expected(item.Tenant)} observed={item.Credential}");
bool valid = records.Length == 2 && records.All(x => x.Credential == Expected(x.Tenant));
Console.WriteLine(valid ? "ISOLATION_OK" : "ISOLATION_BROKEN");
if (mode == "verify") return valid ? 0 : 1;
if (mode == "reproduce") return valid ? 1 : 0;
return 0;

static string Expected(string tenant) => tenant == "A" ? "school-a-secret" : "school-b-secret";

sealed class FakePartner : HttpMessageHandler
{
    private readonly System.Collections.Concurrent.ConcurrentBag<(string Tenant, string Credential)> _records = new();
    public IEnumerable<(string Tenant, string Credential)> Records => _records;
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string tenant = request.Headers.GetValues("X-Tenant").Single();
        string credential = request.Headers.Authorization?.Parameter ?? "<missing>";
        _records.Add((tenant, credential));
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }
}
