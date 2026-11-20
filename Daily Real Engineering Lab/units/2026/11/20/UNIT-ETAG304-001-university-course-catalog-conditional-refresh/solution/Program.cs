using System.Net;
using System.Net.Http.Headers;

var mode = args.Length == 0 ? "repro" : args[0];
var handler = new CatalogHandler();
using var client = new HttpClient(handler) { BaseAddress = new Uri("https://catalog.example.test/") };
var sync = new CatalogSync(client);

if (mode == "repro")
{
    await sync.RefreshAsync();
    Console.WriteLine($"FIRST count={sync.Courses.Count} etag={sync.ETag} status={handler.LastStatus}");
    await sync.RefreshAsync();
    Console.WriteLine($"SECOND count={sync.Courses.Count} etag={sync.ETag} status={handler.LastStatus}");
}
else if (mode == "verify")
{
    await sync.RefreshAsync();
    Check(sync.Courses.SequenceEqual(new[] { "CS101", "NET202" }), "initial catalog");
    Check(sync.ETag == "\"v1\"", "etag captured");
    await sync.RefreshAsync();
    Check(sync.Courses.SequenceEqual(new[] { "CS101", "NET202" }), "unchanged catalog retained");
    Check(handler.ConditionalRequests == 1, "conditional request sent");
    Check(handler.PayloadResponses == 1, "no redundant payload");
    handler.Version = 2;
    await sync.RefreshAsync();
    Check(sync.Courses.SequenceEqual(new[] { "CS101", "NET202", "SEC303" }), "changed catalog replaced");
    Check(sync.ETag == "\"v2\"", "etag advanced");
    handler.FailNext = true;
    try { await sync.RefreshAsync(); throw new Exception("expected upstream error"); }
    catch (HttpRequestException) { }
    Check(sync.Courses.Count == 3 && sync.ETag == "\"v2\"", "failed refresh preserves last good snapshot");
    Console.WriteLine("VERIFY PASS");
}
else throw new ArgumentException("Use repro or verify");

static void Check(bool ok, string description)
{
    if (!ok) throw new Exception("VERIFY FAIL: " + description);
}

sealed class CatalogHandler : HttpMessageHandler
{
    public int Version { get; set; } = 1;
    public bool FailNext { get; set; }
    public int ConditionalRequests { get; private set; }
    public int PayloadResponses { get; private set; }
    public HttpStatusCode LastStatus { get; private set; }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        if (request.RequestUri?.AbsolutePath != "/courses") throw new InvalidOperationException("unexpected route");
        var currentTag = $"\"v{Version}\"";
        if (request.Headers.IfNoneMatch.Count > 0) ConditionalRequests++;
        HttpResponseMessage response;
        if (FailNext)
        {
            FailNext = false;
            response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
        }
        else if (request.Headers.IfNoneMatch.Any(x => x.Tag == currentTag))
        {
            response = new HttpResponseMessage(HttpStatusCode.NotModified);
        }
        else
        {
            PayloadResponses++;
            var payload = Version == 1 ? "CS101,NET202" : "CS101,NET202,SEC303";
            response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(payload) };
        }
        response.Headers.ETag = new EntityTagHeaderValue(currentTag);
        LastStatus = response.StatusCode;
        return Task.FromResult(response);
    }
}

sealed class CatalogSync
{
    private readonly HttpClient _client;
    public IReadOnlyList<string> Courses { get; private set; } = Array.Empty<string>();
    public string? ETag { get; private set; }
    public CatalogSync(HttpClient client) => _client = client;

    public async Task RefreshAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "courses");
        if (ETag is not null) request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(ETag));
        using var response = await _client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            if (ETag is null) throw new HttpRequestException("304 without cached representation");
            return;
        }
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        var parsed = body.Split(',', StringSplitOptions.RemoveEmptyEntries);
        Courses = parsed;
        ETag = response.Headers.ETag?.Tag;
    }
}
