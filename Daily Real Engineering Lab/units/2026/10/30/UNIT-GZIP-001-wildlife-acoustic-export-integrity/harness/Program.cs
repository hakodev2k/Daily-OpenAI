using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using WildlifeArchive;

var mode = args.Length == 1 ? args[0].ToLowerInvariant() : "run";
if (mode is not ("run" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project harness/LabHarness.csproj -- [run|reproduce|verify]");
    return 2;
}

var cases = new (string Name, byte[] Payload)[]
{
    ("acoustic-batch", Encoding.UTF8.GetBytes(string.Join("\n",
        Enumerable.Range(1, 150).Select(i => $"survey=OWL-{i:D4};duration_ms={i * 31};species=nightjar")))),
    ("empty-batch", Array.Empty<byte>())
};

var results = new List<(string Name, Probe Result)>();
foreach (var test in cases)
{
    var compressed = ArchiveExporter.Compress(test.Payload);
    var header = compressed.Length >= 10 && compressed[0] == 0x1f && compressed[1] == 0x8b && compressed[2] == 0x08;
    var footer = compressed.Length >= 18 &&
                 compressed.AsSpan(compressed.Length - 8).SequenceEqual(ExpectedTrailer(test.Payload));
    var roundtrip = TryRoundTrip(compressed, test.Payload);
    results.Add((test.Name, new Probe(header, footer, roundtrip)));
    Console.WriteLine($"{test.Name}: bytes={compressed.Length}, header={header}, footer={footer}, roundtrip={roundtrip}");
}

if (mode == "reproduce")
{
    var reproduced = results.All(x => x.Result.Header) && results.Any(x => !x.Result.Footer);
    Console.WriteLine(reproduced ? "REPRODUCED: GZIP integrity contract violated." : "NOT REPRODUCED: investigate the starter state.");
    return reproduced ? 0 : 1;
}

if (mode == "verify")
{
    var passed = results.All(x => x.Result.Header && x.Result.Footer && x.Result.RoundTrip);
    Console.WriteLine(passed ? "PASS: both GZIP artifacts satisfy integrity and roundtrip." : "FAIL: one or more GZIP artifacts violate the contract.");
    return passed ? 0 : 1;
}

return 0;

static byte[] ExpectedTrailer(ReadOnlySpan<byte> input)
{
    // GZIP trailer: CRC-32 (IEEE reflected polynomial) and ISIZE, both little-endian.
    uint crc = 0xffffffffu;
    foreach (byte b in input)
    {
        crc ^= b;
        for (int bit = 0; bit < 8; bit++)
            crc = (crc & 1u) != 0 ? (crc >> 1) ^ 0xedb88320u : crc >> 1;
    }

    var trailer = new byte[8];
    BinaryPrimitives.WriteUInt32LittleEndian(trailer.AsSpan(0, 4), ~crc);
    BinaryPrimitives.WriteUInt32LittleEndian(trailer.AsSpan(4, 4), unchecked((uint)input.Length));
    return trailer;
}

static bool TryRoundTrip(byte[] gzipBytes, byte[] original)
{
    try
    {
        using var input = new MemoryStream(gzipBytes);
        using var decoder = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        decoder.CopyTo(output);
        return output.ToArray().AsSpan().SequenceEqual(original);
    }
    catch (InvalidDataException)
    {
        return false;
    }
    catch (IOException)
    {
        return false;
    }
}

internal readonly record struct Probe(bool Header, bool Footer, bool RoundTrip);
