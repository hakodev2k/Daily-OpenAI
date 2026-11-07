using System.Net;
using System.Net.Http.Headers;
using System.Text;
using RareBookLab;

const string V1 = """{"edition":1,"books":["Atlas of Stars","Sea Maps"]}""";
const string V2 = """{"edition":2,"books":["Atlas of Stars","Sea Maps","Botanical Plates"]}""";

try
{
    if (args.Length != 1 || (args[0] != "--reproduce" && args[0] != "--verify"))
        throw new ArgumentException("Run with --reproduce or --verify.");

    if (args[0] == "--reproduce")
    {
        using var origin = new CatalogOrigin(V1, "\"edition-1\"");
        using var http = new HttpClient(origin);
        var client = new CatalogClient(http);

        var first = await client.GetCatalogAsync();
        client.EvictBodyForMemoryPressure();
        var second = await client.GetCatalogAsync();

        Check(first == V1, "First fetch should load the full catalog.");
        Check(second == "", "Starter symptom is not reproduced: expected empty catalog after eviction.");
        Check(origin.StatusCodes.SequenceEqual(new[] { 200, 304 }), "Expected 200 then 304.");
        Check(origin.ConditionalRequests.SequenceEqual(new[] { false, true }), "Expected unconditional then conditional request.");
        Console.WriteLine("REPRODUCED: origin=200,304; catalog becomes empty after memory pressure.");
    }
    else
    {
        using var origin = new CatalogOrigin(V1, "\"edition-1\"");
        using var http = new HttpClient(origin);
        var client = new CatalogClient(http);

        Check(await client.GetCatalogAsync() == V1, "Cold fetch must return edition 1.");
        client.EvictBodyForMemoryPressure();
        Check(await client.GetCatalogAsync() == V1, "Eviction must not produce an empty catalog.");
        Check(origin.StatusCodes.SequenceEqual(new[] { 200, 200 }), "Without a usable local representation, origin should return a full 200.");
        Check(origin.ConditionalRequests.SequenceEqual(new[] { false, false }), "After eviction, the request must not carry a validator for an unavailable body.");

        Check(await client.GetCatalogAsync() == V1, "A valid unchanged catalog must survive revalidation.");
        Check(origin.StatusCodes.Last() == 304 && origin.ConditionalRequests.Last(), "With a resident body, conditional 304 should save bandwidth.");

        origin.CurrentBody = V2;
        origin.CurrentEtag = "\"edition-2\"";
        Check(await client.GetCatalogAsync() == V2, "Changed origin version must refresh the local catalog.");
        Check(origin.StatusCodes.Last() == 200 && origin.ConditionalRequests.Last(), "Changed version should return full 200 to conditional request.");
        Check(await client.GetCatalogAsync() == V2, "Updated catalog should survive the next 304.");
        Check(origin.StatusCodes.Last() == 304, "Updated validator should support a later 304.");

        using var anomalous = new CatalogOrigin(V1, "\"edition-1\"") { AlwaysReturn304 = true };
        using var anomalousHttp = new HttpClient(anomalous);
        var anomalousClient = new CatalogClient(anomalousHttp);
        bool rejected = false;
        try { await anomalousClient.GetCatalogAsync(); }
        catch (InvalidOperationException) { rejected = true; }
        Check(rejected, "Unexpected 304 without a local representation must not be silently accepted.");

        Console.WriteLine("VERIFIED: eviction, 304 cache hit, version rotation, anomalous 304.");
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"CHECK FAILED: {ex.Message}");
    Environment.ExitCode = 1;
}

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed class CatalogOrigin : HttpMessageHandler
{
    public CatalogOrigin(string body, string etag)
    {
        CurrentBody = body;
        CurrentEtag = etag;
    }

    public string CurrentBody { get; set; }
    public string CurrentEtag { get; set; }
    public bool AlwaysReturn304 { get; set; }
    public List<int> StatusCodes { get; } = new();
    public List<bool> ConditionalRequests { get; } = new();

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (request.Method != HttpMethod.Get)
            throw new InvalidOperationException("Fixture only supports GET.");

        var hasValidator = request.Headers.IfNoneMatch.Count > 0;
        var matches = request.Headers.IfNoneMatch.Any(x => x.Tag == CurrentEtag);
        ConditionalRequests.Add(hasValidator);

        var status = AlwaysReturn304 || matches
            ? HttpStatusCode.NotModified
            : HttpStatusCode.OK;
        StatusCodes.Add((int)status);

        var response = new HttpResponseMessage(status);
        response.Headers.ETag = new EntityTagHeaderValue(CurrentEtag);
        if (status == HttpStatusCode.OK)
            response.Content = new StringContent(CurrentBody, Encoding.UTF8, "application/json");
        return Task.FromResult(response);
    }
}
