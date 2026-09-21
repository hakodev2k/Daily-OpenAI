record PartialResponse(int StatusCode, string ContentRange, byte[] Body);

static PartialResponse BuildPartial(byte[] resource, int start, int requestedEnd)
{
    if (start < 0 || start >= resource.Length || requestedEnd < start)
        return new PartialResponse(416, $"bytes */{resource.Length}", []);

    var actualEnd = Math.Min(requestedEnd, resource.Length - 1);
    var body = resource[start..(actualEnd + 1)];
    return new PartialResponse(206, $"bytes {start}-{actualEnd}/{resource.Length}", body);
}

var resource = Enumerable.Range(0, 100).Select(i => (byte)i).ToArray();
var response = BuildPartial(resource, 90, 120);
Console.WriteLine($"{response.StatusCode} {response.ContentRange} bodyLength={response.Body.Length}");