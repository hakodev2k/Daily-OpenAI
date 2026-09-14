using System.Net;
using System.Net.Http.Json;

var handler = new SequencedHandler();
using var client = new HttpClient(handler)
{
    BaseAddress = new Uri("https://partner.local/")
};

var command = new PartnerCommand("order-42", 125_000);

for (var attempt = 1; attempt <= 2; attempt++)
{
    using var request = new HttpRequestMessage(HttpMethod.Post, "commands")
    {
        Content = JsonContent.Create(command)
    };

    using var response = await client.SendAsync(request);

    if (response.StatusCode == HttpStatusCode.ServiceUnavailable && attempt < 2)
    {
        continue;
    }

    Console.WriteLine($"RESULT={(response.IsSuccessStatusCode ? "OK" : "FAILED")}");
    Console.WriteLine($"STATUS={(int)response.StatusCode}");
    Console.WriteLine($"ATTEMPTS={handler.RequestCount}");
    return;
}

sealed record PartnerCommand(string OrderId, int Amount);

sealed class SequencedHandler : HttpMessageHandler
{
    public int RequestCount { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        RequestCount++;

        var status = RequestCount == 1
            ? HttpStatusCode.ServiceUnavailable
            : HttpStatusCode.OK;

        return Task.FromResult(new HttpResponseMessage(status)
        {
            Content = new StringContent(status == HttpStatusCode.OK ? "accepted" : "temporary")
        });
    }
}
