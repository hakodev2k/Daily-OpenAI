using System.Net;
using System.Text.Json;

public sealed record ObservedCall(string Method, string Path, string Body);

public sealed class FakePartnerHandler : HttpMessageHandler
{
    private readonly string _prefix;
    public List<ObservedCall> Calls { get; } = new();
    public FakePartnerHandler(string prefix) => _prefix = prefix;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath ?? "";
        var body = request.Content is null ? "" : await request.Content.ReadAsStringAsync(cancellationToken);
        Calls.Add(new ObservedCall(request.Method.Method, path, body));

        if (request.Method == HttpMethod.Post && path == _prefix + "/labels/submit")
        {
            try
            {
                using var json = JsonDocument.Parse(body);
                if (json.RootElement.TryGetProperty("shipmentId", out var shipmentId)
                    && shipmentId.GetString() == "SHIP-42")
                    return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (JsonException)
            {
                // Fake partner rejects invalid JSON.
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
        if (request.Method == HttpMethod.Get && path == _prefix + "/labels/status/SHIP-42")
            return new HttpResponseMessage(HttpStatusCode.OK);

        return new HttpResponseMessage(HttpStatusCode.NotFound);
    }
}
