using System.Net;
using System.Text;

namespace CanalGate.Export;

internal sealed class TrackingHandler : HttpMessageHandler
{
    public const string ExpectedCsv =
        "gateId,levelCm\ncanal-17,142\ncanal-17,143\n";

    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public TrackedStream? LastStream { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request.Method != HttpMethod.Get ||
            request.RequestUri?.AbsolutePath != "/exports/canal-17")
            throw new InvalidOperationException("Unexpected request route.");

        LastStream = new TrackedStream(Encoding.UTF8.GetBytes(ExpectedCsv));
        var response = new HttpResponseMessage(StatusCode)
        {
            Content = new StreamContent(LastStream)
        };
        response.Content.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
        return Task.FromResult(response);
    }
}

internal sealed class TrackedStream : MemoryStream
{
    public TrackedStream(byte[] bytes) : base(bytes, writable: false) { }

    public bool IsDisposed { get; private set; }

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        base.Dispose(disposing);
    }

    public override ValueTask DisposeAsync()
    {
        IsDisposed = true;
        return base.DisposeAsync();
    }
}
