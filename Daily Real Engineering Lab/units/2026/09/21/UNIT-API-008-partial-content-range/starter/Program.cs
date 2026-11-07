record PartialResponse(int StatusCode, string ContentRange, byte[] Body);

static PartialResponse BuildPartial(byte[] resource, int start, int requestedEnd)
{
    if (start < 0 || start >= resource.Length || requestedEnd < start)
        return new PartialResponse(416, $"bytes */{resource.Length}", []);

    var actualEnd = Math.Min(requestedEnd, resource.Length - 1);
    var body = resource[start..(actualEnd + 1)];

    // Investigation note: compare every response field with the bytes actually returned.
    return new PartialResponse(
        206,
        $"bytes {start}-{requestedEnd}/{resource.Length}",
        body);
}

var resource = Enumerable.Range(0, 100).Select(i => (byte)i).ToArray();

if (args.Contains("--reproduce"))
{
    var response = BuildPartial(resource, 90, 120);
    Console.WriteLine($"status={response.StatusCode}");
    Console.WriteLine($"Content-Range: {response.ContentRange}");
    Console.WriteLine($"bodyLength={response.Body.Length}");
    return response.ContentRange == "bytes 90-120/100" && response.Body.Length == 10 ? 0 : 2;
}

if (args.Contains("--verify"))
{
    var cases = new[] { (Start: 0, End: 9, Expected: "bytes 0-9/100", Length: 10), (Start: 90, End: 120, Expected: "bytes 90-99/100", Length: 10), (Start: 99, End: 150, Expected: "bytes 99-99/100", Length: 1) };
    foreach (var test in cases)
    {
        var response = BuildPartial(resource, test.Start, test.End);
        if (response.StatusCode != 206 || response.ContentRange != test.Expected || response.Body.Length != test.Length)
        {
            Console.Error.WriteLine($"FAIL start={test.Start} end={test.End} range={response.ContentRange} length={response.Body.Length}");
            return 3;
        }
    }
    Console.WriteLine("VERIFY_PASS");
    return 0;
}

var sample = BuildPartial(resource, 10, 19);
Console.WriteLine($"{sample.StatusCode} {sample.ContentRange} bodyLength={sample.Body.Length}");
return 0;