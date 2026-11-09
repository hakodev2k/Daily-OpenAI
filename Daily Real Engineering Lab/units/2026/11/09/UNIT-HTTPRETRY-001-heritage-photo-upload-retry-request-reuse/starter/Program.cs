using System.Net;
using System.Net.Http;
using System.Text;

var handler = new ScriptedGateway();
using var client = new HttpClient(handler);
var uploader = new PhotoUploader(client);
try
{
    var response = await uploader.UploadAsync("artifact-42", Encoding.UTF8.GetBytes("PHOTO-BYTES"));
    Console.WriteLine($"RESULT={(int)response.StatusCode}; RECEIVED={handler.RequestCount}");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR={ex.GetType().Name}; RECEIVED={handler.RequestCount}");
    Environment.ExitCode = 2;
}

public sealed class PhotoUploader(HttpClient client)
{
    public async Task<HttpResponseMessage> UploadAsync(string photoId, byte[] photo)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://archive.invalid/photos");
        request.Headers.TryAddWithoutValidation("Idempotency-Key", photoId);
        request.Content = new ByteArrayContent(photo);
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.ServiceUnavailable || attempt == 1)
                return response;
            response.Dispose();
        }
        throw new InvalidOperationException("Unreachable");
    }
}

public sealed class ScriptedGateway : HttpMessageHandler
{
    public int RequestCount { get; private set; }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestCount++;
        var payload = await request.Content!.ReadAsByteArrayAsync(cancellationToken);
        if (request.Headers.GetValues("Idempotency-Key").Single() != "artifact-42" || Encoding.UTF8.GetString(payload) != "PHOTO-BYTES")
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        return new HttpResponseMessage(RequestCount == 1 ? HttpStatusCode.ServiceUnavailable : HttpStatusCode.OK);
    }
}

