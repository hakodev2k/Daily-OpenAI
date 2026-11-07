using System.IO.Compression;
using System.Net;

const long maxDecompressedBytes = 1_000_000;

var handler = new SocketsHttpHandler
{
    AutomaticDecompression = DecompressionMethods.GZip
};

using var client = new HttpClient(handler);
using var response = await client.GetAsync("http://127.0.0.1:5088/export", HttpCompletionOption.ResponseHeadersRead);
response.EnsureSuccessStatusCode();

await using var source = await response.Content.ReadAsStreamAsync();
await using var destination = new MemoryStream();

var buffer = new byte[16 * 1024];
long total = 0;

while (true)
{
    var read = await source.ReadAsync(buffer);
    if (read == 0) break;

    total += read;
    if (total > maxDecompressedBytes)
        throw new InvalidDataException($"Decompressed response exceeded {maxDecompressedBytes} bytes.");

    await destination.WriteAsync(buffer.AsMemory(0, read));
}

Console.WriteLine($"Accepted decompressed bytes: {total}");
