using System.Net;
using System.Net.Http.Headers;
using System.Text;

var mode = args.FirstOrDefault() ?? "run";
byte[] canonical = Encoding.ASCII.GetBytes("ABCDEFGHIJKL");
byte[] prefix = canonical[..4];
using var client = new HttpClient(new LocalRangeHandler(canonical));
using var response = await client.GetAsync("https://maps.invalid/tiles/zone-7", HttpCompletionOption.ResponseHeadersRead);
byte[] incoming = await response.Content.ReadAsByteArrayAsync();
Console.WriteLine($"status={(int)response.StatusCode} prefix={prefix.Length} received={incoming.Length} range={response.Content.Headers.ContentRange}");
byte[] assembled = ResumeAssembler.Append(prefix, response, incoming);
Console.WriteLine($"assembled={Encoding.ASCII.GetString(assembled)} expected={Encoding.ASCII.GetString(canonical)}");
bool ok = assembled.SequenceEqual(canonical);
if (mode == "verify")
{
    // An honest 206 must be accepted; a contradictory 206 must be rejected.
    using var good = new HttpResponseMessage(HttpStatusCode.PartialContent) { Content = new ByteArrayContent(canonical[4..]) };
    good.Content.Headers.ContentRange = new ContentRangeHeaderValue(4, 11, 12);
    byte[] valid = ResumeAssembler.Append(prefix, good, canonical[4..]);
    if (!valid.SequenceEqual(canonical)) throw new Exception("Valid range regression");
    bool rejected = false;
    try { ResumeAssembler.Append(prefix, response, incoming); }
    catch (InvalidDataException) { rejected = true; }
    if (!rejected) throw new Exception("Mismatched range was accepted");
    Console.WriteLine("VERIFY PASS: valid range accepted; contradictory range rejected");
}
else if (mode == "reproduce")
{
    if (ok) throw new Exception("Starter did not reproduce the expected integrity failure");
    Console.WriteLine("REPRODUCED: assembled payload differs from canonical");
}
else Console.WriteLine(ok ? "integrity=OK" : "integrity=FAILED");

internal static class ResumeAssembler
{
    public static byte[] Append(byte[] existing, HttpResponseMessage response, byte[] incoming)
    {
        if (response.StatusCode != HttpStatusCode.PartialContent)
            throw new InvalidDataException("Expected partial response");
        using var buffer = new MemoryStream();
        buffer.Write(existing);
        buffer.Write(incoming);
        return buffer.ToArray();
    }
}

internal sealed class LocalRangeHandler(byte[] canonical) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Deterministic fixture: gateway returns a valid 206 shape but starts one byte too early.
        var reply = new HttpResponseMessage(HttpStatusCode.PartialContent)
        { Content = new ByteArrayContent(canonical[3..]) };
        reply.Content.Headers.ContentRange = new ContentRangeHeaderValue(3, 11, 12);
        return Task.FromResult(reply);
    }
}
