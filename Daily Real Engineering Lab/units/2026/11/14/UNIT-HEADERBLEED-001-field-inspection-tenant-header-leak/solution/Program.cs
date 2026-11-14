using System.Net;
using System.Net.Http.Headers;

var mode = args.Length == 0 ? "reproduce" : args[0].ToLowerInvariant();
if (mode is not ("reproduce" or "verify"))
{
    Console.Error.WriteLine("Dùng: dotnet run -- reproduce|verify");
    return 2;
}

using var handler = new CaptureHandler();
using var client = new HttpClient(handler) { BaseAddress = new Uri("https://inspections.example.invalid/") };
var gateway = new UploadGateway(client);
var firstReady = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var releaseFirst = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var first = gateway.UploadAsync("tenant-a", "token-a", firstReady, releaseFirst.Task);
await firstReady.Task.WaitAsync(TimeSpan.FromSeconds(5));
var second = gateway.UploadAsync("tenant-b", "token-b", null, Task.CompletedTask);
await second.WaitAsync(TimeSpan.FromSeconds(5));
releaseFirst.SetResult();
await first.WaitAsync(TimeSpan.FromSeconds(5));

var records = handler.Records.OrderBy(x => x.Tenant).ToArray();
foreach (var record in records)
    Console.WriteLine($"tenant={record.Tenant} actual={record.Token} expected=token-{record.Tenant[^1]}");
bool valid = records.Length == 2 && records.All(x => x.Token == "token-" + x.Tenant[^1]);
Console.WriteLine(valid ? "ISOLATION_PASS" : "ISOLATION_FAIL");
return mode == "reproduce" ? (valid ? 1 : 0) : (valid ? 0 : 1);

sealed class UploadGateway(HttpClient client)
{
    public async Task UploadAsync(string tenant, string token, TaskCompletionSource? ready, Task wait)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "upload/" + tenant);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        ready?.SetResult();
        await wait;
        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}

sealed class CaptureHandler : HttpMessageHandler
{
    private readonly System.Collections.Concurrent.ConcurrentBag<(string Tenant, string Token)> records = new();
    public (string Tenant, string Token)[] Records => records.ToArray();
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string tenant = request.RequestUri!.Segments.Last();
        string token = request.Headers.Authorization?.Parameter ?? "<missing>";
        records.Add((tenant, token));
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }
}
