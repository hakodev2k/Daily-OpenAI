using System.Security.Cryptography;

var payload = Enumerable.Range(0, 101).Select(i => (byte)((i * 37 + 11) % 256)).ToArray();
var cases = new (int Start, int End)[] { (0, 31), (32, 63), (64, 100) };
var chunks = cases.Select(c => Segment(payload, c.Start, c.End)).ToArray();
var joined = chunks.SelectMany(x => x).ToArray();
Console.WriteLine($"Original={payload.Length} Joined={joined.Length}");
foreach (var (range, i) in cases.Select((r, i) => (r, i)))
    Console.WriteLine($"bytes={range.Start}-{range.End}/{payload.Length}; actualLength={chunks[i].Length}");
Console.WriteLine($"Original SHA256={Convert.ToHexString(SHA256.HashData(payload))}");
Console.WriteLine($"Joined   SHA256={Convert.ToHexString(SHA256.HashData(joined))}");
bool valid = joined.SequenceEqual(payload);
Console.WriteLine(valid ? "PASS" : "FAIL: assembled bytes differ");
if (args.Contains("--verify"))
{
    var checks = new (int Start, int End)[] { (0, 0), (100, 100), (0, 100), (30, 31), (32, 63) };
    foreach (var (start, end) in checks)
    {
        var expected = payload.Skip(start).Take(end - start + 1).ToArray();
        if (!Segment(payload, start, end).SequenceEqual(expected))
        {
            Console.Error.WriteLine($"FAIL boundary {start}-{end}");
            Environment.Exit(1);
        }
    }
    foreach (var (start, end) in new (int, int)[] { (-1, 2), (2, 101), (9, 8) })
    {
        try { _ = Segment(payload, start, end); Console.Error.WriteLine("FAIL invalid range accepted"); Environment.Exit(1); }
        catch (ArgumentOutOfRangeException) { }
    }
    if (!valid) Environment.Exit(1);
    Console.WriteLine("PASS all regression checks");
}

static byte[] Segment(byte[] data, int start, int end)
{
    if (start < 0 || end < start || end >= data.Length) throw new ArgumentOutOfRangeException(nameof(start));
    // Range request is recorded as "bytes=start-end" in gateway logs.
    var length = end - start;
    return data.AsSpan(start, length).ToArray();
}
