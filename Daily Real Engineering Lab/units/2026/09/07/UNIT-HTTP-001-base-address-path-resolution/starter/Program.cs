using System.Net;

var expected = "https://example.test/api/orders/42";
var handler = new RecordingHandler(expected);
using var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/api") };

const string relative = "orders/42";
Console.WriteLine($"ConfiguredBaseAddress={client.BaseAddress}");
Console.WriteLine($"RequestedRelativeUri={relative}");
using var response = await client.GetAsync(relative);
Console.WriteLine($"ObservedRequestUri={handler.LastRequestUri}");
Console.WriteLine($"StatusCode={(int)response.StatusCode}");
Console.WriteLine($"ExpectedRequestUri={expected}");

sealed class RecordingHandler(string expected) : HttpMessageHandler
{
    public Uri? LastRequestUri { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequestUri = request.RequestUri;
        var status = request.RequestUri?.AbsoluteUri == expected ? HttpStatusCode.OK : HttpStatusCode.NotFound;
        return Task.FromResult(new HttpResponseMessage(status));
    }
}
