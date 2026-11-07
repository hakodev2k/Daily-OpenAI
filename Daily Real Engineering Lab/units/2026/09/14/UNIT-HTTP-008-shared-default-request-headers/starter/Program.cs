using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

var handler = new RecordingHandler();
using var httpClient = new HttpClient(handler)
{
    BaseAddress = new Uri("https://partner.invalid/")
};

var gate = new AsyncGate(2);
var partner = new PartnerClient(httpClient, gate);

var tenantA = partner.GetOrdersAsync("tenant-a", "token-A");
var tenantB = partner.GetOrdersAsync("tenant-b", "token-B");

var results = await Task.WhenAll(tenantA, tenantB);

foreach (var result in results.OrderBy(x => x.Tenant))
{
    Console.WriteLine($"{result.Tenant}: expected={result.ExpectedAuthorization}, sent={result.SentAuthorization}, match={result.Match}");
}

if (args.Contains("--verify", StringComparer.OrdinalIgnoreCase))
{
    if (results.Any(x => !x.Match))
    {
        Console.Error.WriteLine("VERIFY_FAILED: request context leaked across tenants.");
        Environment.ExitCode = 1;
        return;
    }

    Console.WriteLine("VERIFY_OK: each tenant request preserved its own authorization context.");
    return;
}

if (results.All(x => x.Match))
{
    Console.Error.WriteLine("REPRODUCE_FAILED: expected at least one mismatched authorization value.");
    Environment.ExitCode = 2;
    return;
}

Console.WriteLine("REPRODUCE_OK: observed cross-tenant authorization mismatch.");

internal sealed class PartnerClient
{
    private readonly HttpClient _httpClient;
    private readonly AsyncGate _gate;

    public PartnerClient(HttpClient httpClient, AsyncGate gate)
    {
        _httpClient = httpClient;
        _gate = gate;
    }

    public async Task<CallResult> GetOrdersAsync(string tenant, string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Investigation note:
        // Both logical requests pause here before the actual send.
        // What state belongs to the individual request, and what state is shared?
        await _gate.SignalAndWaitAsync();

        using var response = await _httpClient.GetAsync($"orders/{tenant}");
        var json = await response.Content.ReadAsStringAsync();
        var echoed = JsonSerializer.Deserialize<EchoResponse>(json)!;
        var expected = $"Bearer {token}";

        return new CallResult(tenant, expected, echoed.Authorization, expected == echoed.Authorization);
    }
}

internal sealed class RecordingHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authorization = request.Headers.Authorization?.ToString() ?? "<missing>";
        var payload = JsonSerializer.Serialize(new EchoResponse(authorization));

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(payload)
        });
    }
}

internal sealed class AsyncGate
{
    private readonly int _requiredSignals;
    private readonly TaskCompletionSource<bool> _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _signals;

    public AsyncGate(int requiredSignals) => _requiredSignals = requiredSignals;

    public Task SignalAndWaitAsync()
    {
        if (Interlocked.Increment(ref _signals) == _requiredSignals)
        {
            _ready.TrySetResult(true);
        }

        return _ready.Task;
    }
}

internal sealed record EchoResponse(string Authorization);
internal sealed record CallResult(string Tenant, string ExpectedAuthorization, string SentAuthorization, bool Match);
