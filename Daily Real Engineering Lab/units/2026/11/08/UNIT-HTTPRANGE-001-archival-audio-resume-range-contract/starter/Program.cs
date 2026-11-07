using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;

var original = Enumerable.Range(0, 128).Select(i => (byte)(i % 251)).ToArray();
var mode = args.Length == 0 ? "ignored" : args[0];
if (mode is not ("honored" or "ignored")) throw new ArgumentException("Mode: honored|ignored");
var existing = original[..32];
using var handler = new FakeArchiveHandler(original, mode == "honored");
using var client = new HttpClient(handler);
using var request = new HttpRequestMessage(HttpMethod.Get, "https://archive.invalid/audio/42");
request.Headers.Range = new RangeHeaderValue(existing.Length, null);
using var response = await client.SendAsync(request);
response.EnsureSuccessStatusCode();
using var output = new MemoryStream();
await output.WriteAsync(existing);
await response.Content.CopyToAsync(output);
var actual = output.ToArray();
Console.WriteLine($"mode={mode} status={(int)response.StatusCode} contentRange={response.Content.Headers.ContentRange?.ToString() ?? "none"} bytes={actual.Length} expected={original.Length} shaMatch={SHA256.HashData(actual).SequenceEqual(SHA256.HashData(original))}");
if (!actual.SequenceEqual(original)) Environment.ExitCode = 2;

sealed class FakeArchiveHandler(byte[] source, bool honorRange) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var start = (int)(request.Headers.Range?.Ranges.Single().From ?? 0);
        var partial = honorRange && start < source.Length;
        var bytes = partial ? source[start..] : source;
        var result = new HttpResponseMessage(partial ? HttpStatusCode.PartialContent : HttpStatusCode.OK)
        { Content = new ByteArrayContent(bytes) };
        if (partial) result.Content.Headers.ContentRange = new ContentRangeHeaderValue(start, source.Length - 1, source.Length);
        return Task.FromResult(result);
    }
}
