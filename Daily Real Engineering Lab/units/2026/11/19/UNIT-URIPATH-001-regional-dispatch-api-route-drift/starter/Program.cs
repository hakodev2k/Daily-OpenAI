using System.Net;
using System.Text.Json;
var mode = args.FirstOrDefault() ?? "run";
if (mode is not ("run" or "reproduce" or "verify")) return 2;
var regions = new[] { "north", "south" };
int failed = 0;
foreach (var region in regions)
{
    var expected = $"/api/tenants/{region}/assignments?priority=urgent";
    using var gateway = new Gateway(expected);
    using var client = new HttpClient(gateway) { BaseAddress = new Uri($"https://partner.test/api/tenants/{region}/") };
    client.DefaultRequestHeaders.Add("X-Partner-Key", "lab-key");
    var path = "/assignments?priority=urgent";
    using var response = await client.PostAsync(path, new StringContent("{}"));
    bool pass = response.StatusCode == HttpStatusCode.Accepted && gateway.AuthOk && gateway.Actual?.PathAndQuery == expected;
    if (!pass) failed++;
    Console.WriteLine(JsonSerializer.Serialize(new { region, uri = gateway.Actual?.AbsoluteUri, expected, status = (int)response.StatusCode, auth = gateway.AuthOk, pass }));
}
return mode switch { "run" => 0, "reproduce" => failed > 0 ? 0 : 1, _ => failed == 0 ? 0 : 1 };
sealed class Gateway(string expected) : HttpMessageHandler
{
    public Uri? Actual { get; private set; }
    public bool AuthOk { get; private set; }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        Actual = request.RequestUri;
        AuthOk = request.Headers.TryGetValues("X-Partner-Key", out var values) && values.SingleOrDefault() == "lab-key";
        var ok = request.Method == HttpMethod.Post && Actual?.PathAndQuery == expected && AuthOk;
        return Task.FromResult(new HttpResponseMessage(ok ? HttpStatusCode.Accepted : HttpStatusCode.NotFound));
    }
}
