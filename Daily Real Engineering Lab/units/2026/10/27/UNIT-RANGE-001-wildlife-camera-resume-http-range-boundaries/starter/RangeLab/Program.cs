using System.Net;
using System.Net.Http.Headers;
using RangeLab;

var brokenFixtures = 0;
foreach (var size in new[] { 10, 12, 1 })
{
    var original = Enumerable.Range(0, size).Select(n => (byte)(0x30 + n)).ToArray();
    var handler = new LocalRangeServer(original);
    using var client = new HttpClient(handler);
    var downloader = new ChunkDownloader(client, chunkSize: 4);
    var downloaded = await downloader.DownloadAsync(original.Length);
    var matches = downloaded.SequenceEqual(original);
    if (!matches) brokenFixtures++;

    var firstMismatch = Enumerable.Range(0, Math.Min(original.Length, downloaded.Length))
        .Where(i => original[i] != downloaded[i])
        .Select(i => i.ToString())
        .FirstOrDefault() ?? (original.Length == downloaded.Length ? "none" : "length-boundary");
    Console.WriteLine($"sourceBytes={original.Length}; downloadedBytes={downloaded.Length}; sameBytes={matches}; firstMismatch={firstMismatch}");
    Console.WriteLine($"requests={string.Join(", ", handler.Requests)}");
    Console.WriteLine($"hex={Convert.ToHexString(downloaded)}");
}

var mode = args.FirstOrDefault() ?? "--observe";
return mode switch
{
    "--expect-broken" => brokenFixtures == 2 ? 0 : 1,
    "--expect-fixed" => brokenFixtures == 0 ? 0 : 1,
    "--observe" => 0,
    _ => 2
};

sealed class LocalRangeServer : HttpMessageHandler
{
    private readonly byte[] _source;
    public List<string> Requests { get; } = new();

    public LocalRangeServer(byte[] source) => _source = source;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var range = request.Headers.Range?.Ranges.SingleOrDefault();
        if (request.Method != HttpMethod.Get ||
            range?.From is not long from || range.To is not long to ||
            from < 0 || to < from || to >= _source.LongLength)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.RequestedRangeNotSatisfiable));
        }

        Requests.Add($"bytes={from}-{to}");
        var bytes = _source.AsSpan((int)from, checked((int)(to - from + 1))).ToArray();
        var response = new HttpResponseMessage(HttpStatusCode.PartialContent)
        {
            Content = new ByteArrayContent(bytes)
        };
        response.Content.Headers.ContentRange = new ContentRangeHeaderValue(from, to, _source.LongLength);
        return Task.FromResult(response);
    }
}
