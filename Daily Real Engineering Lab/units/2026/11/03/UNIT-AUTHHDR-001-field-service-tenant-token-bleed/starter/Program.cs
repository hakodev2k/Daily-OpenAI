using System.Net;
using System.Net.Http.Headers;

var mode = args.Length == 1 ? args[0] : "reproduce";
if (mode is not ("reproduce" or "verify")) { Console.Error.WriteLine("Usage: dotnet run -- [reproduce|verify]"); return 2; }
var gateway = new RecordingGateway();
using var client = new HttpClient(gateway) { BaseAddress = new Uri("https://partner.invalid/") };
var dispatcher = new DiagnosticDispatcher(client);
if (mode == "reproduce")
{
    gateway.ReleaseImmediately = true;
    await dispatcher.SendAsync("tenant-A", "token-A");
    await dispatcher.SendAsync("tenant-B", "token-B");
    if (!gateway.Observed.SequenceEqual(new[] { "tenant-A:token-A", "tenant-B:token-B" })) return 3;
    Console.WriteLine("Sequential baseline PASS");
    gateway.Observed.Clear();
}
gateway.ReleaseImmediately = false;
var first = dispatcher.SendAsync("tenant-A", "token-A");
await gateway.WaitForFirstAsync();
var second = dispatcher.SendAsync("tenant-B", "token-B");
await gateway.WaitForSecondAsync();
gateway.Release();
await Task.WhenAll(first, second);
foreach (var row in gateway.Observed) Console.WriteLine(row);
var expected = new[] { "tenant-A:token-A", "tenant-B:token-B" };
bool valid = gateway.Observed.OrderBy(x => x).SequenceEqual(expected.OrderBy(x => x));
Console.WriteLine(valid ? "Tenant isolation PASS" : "Tenant isolation FAILED");
return mode == "reproduce" ? (valid ? 4 : 0) : (valid ? 0 : 5);

sealed class RecordingGateway : HttpMessageHandler
{
    readonly TaskCompletionSource first = new(TaskCreationOptions.RunContinuationsAsynchronously);
    readonly TaskCompletionSource second = new(TaskCreationOptions.RunContinuationsAsynchronously);
    readonly TaskCompletionSource release = new(TaskCreationOptions.RunContinuationsAsynchronously);
    int arrivals;
    public bool ReleaseImmediately { get; set; }
    public List<string> Observed { get; } = new();
    public Task WaitForFirstAsync() => first.Task;
    public Task WaitForSecondAsync() => second.Task;
    public void Release() => release.TrySetResult();
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var ordinal = Interlocked.Increment(ref arrivals);
        if (!ReleaseImmediately)
        {
            if (ordinal == 3) first.TrySetResult();
            if (ordinal == 4) second.TrySetResult();
            await release.Task.WaitAsync(cancellationToken);
        }
        var tenant = request.RequestUri!.Segments.Last();
        var token = request.Headers.Authorization?.Parameter ?? "(missing)";
        lock (Observed) Observed.Add(tenant + ":" + token);
        return new HttpResponseMessage(HttpStatusCode.OK);
    }
}
sealed class DiagnosticDispatcher(HttpClient client)
{
    public async Task SendAsync(string tenant, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var request = new HttpRequestMessage(HttpMethod.Post, "diagnostics/" + tenant);
        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
