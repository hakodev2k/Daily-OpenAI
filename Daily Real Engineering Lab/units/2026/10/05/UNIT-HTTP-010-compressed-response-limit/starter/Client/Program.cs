using System.IO.Compression;
using System.Net;
using System.Text;

const int configuredWireLimitBytes = 1_000_000;

var handler = new SocketsHttpHandler
{
    AutomaticDecompression = DecompressionMethods.GZip
};

using var client = new HttpClient(handler);
using var response = await client.GetAsync("http://127.0.0.1:5088/export");
response.EnsureSuccessStatusCode();

var advertisedLength = response.Content.Headers.ContentLength;
Console.WriteLine($"Content-Encoding after handler: {string.Join(",", response.Content.Headers.ContentEncoding)}");
Console.WriteLine($"Content-Length after handler: {advertisedLength?.ToString() ?? "<none>"}");

if (advertisedLength is > configuredWireLimitBytes)
{
    throw new InvalidOperationException("Response exceeds configured limit.");
}

var body = await response.Content.ReadAsByteArrayAsync();
Console.WriteLine($"Bytes materialized in memory: {body.Length}");

if (body.Length > configuredWireLimitBytes)
{
    Console.WriteLine("OBSERVED: materialized payload is larger than the configured limit.");
}

Console.WriteLine($"Checksum marker: {Encoding.UTF8.GetString(body.AsSpan(0, Math.Min(16, body.Length)))}");
