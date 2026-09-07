using System.Net;

var recorder = new RecordingHandler();
using var client = new HttpClient(recorder)
{
    BaseAddress = new Uri("https://fulfillment.local/gateway/v1/")
};

var requestTarget = "/orders/42";
Console.WriteLine($"BaseAddress={client.BaseAddress}");
Console.WriteLine($"RequestTarget={requestTarget}");

var response = await client.GetAsync(requestTarget);

Console.WriteLine($"FinalUri={recorder.LastRequestUri}");
Console.WriteLine($"Status={(int)response.StatusCode}");

sealed class RecordingHandler : HttpMessageHandler
{
    public Uri? LastRequestUri { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequestUri = request.RequestUri;
        var expected = new Uri("https://fulfillment.local/gateway/v1/orders/42");
        var status = LastRequestUri == expected ? HttpStatusCode.OK : HttpStatusCode.NotFound;
        return Task.FromResult(new HttpResponseMessage(status));
    }
}
