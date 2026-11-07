using System.Text;
using ChunkedTextLab;

// Contract harness. Edit ChunkedNoteDecoder.cs only.
internal static class Program
{
    private sealed record Probe(string Name, bool Passed, bool Rejected, int ReplacementChars, int Chunks);

    private static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "--run" : args[0];
        if (args.Length > 1 || mode is not ("--run" or "--reproduce" or "--verify"))
        {
            Console.Error.WriteLine("Usage: --run | --reproduce | --verify");
            return 2;
        }

        const string note = "Đà Nẵng: xe đạp cần sửa phanh.";
        byte[] payload = Encoding.UTF8.GetBytes(note);
        var results = new[]
        {
            Valid("one-byte", note, Split(payload, Enumerable.Repeat(1, payload.Length).ToArray())),
            Valid("mixed", note, Split(payload, 4, 1, 2, 5, 1, 3)),
            Valid("ascii", "Bike-42 OK", Split(Encoding.UTF8.GetBytes("Bike-42 OK"), 3, 2)),
            Invalid("invalid-sequence", new ReadOnlyMemory<byte>[]
            {
                new byte[] { (byte)'A', 0xC3 },
                new byte[] { 0x28 }
            }),
            Invalid("truncated-sequence", new ReadOnlyMemory<byte>[]
            {
                new byte[] { (byte)'B', 0xE1 }
            })
        };

        foreach (var r in results)
            Console.WriteLine($"{r.Name}: correct={r.Passed} rejected={r.Rejected} replacementChars={r.ReplacementChars} chunks={r.Chunks}");

        if (mode == "--run") return 0;

        if (mode == "--reproduce")
        {
            bool observed = !results[0].Passed && !results[1].Passed &&
                            results[2].Passed && !results[3].Passed && !results[4].Passed;
            Console.WriteLine(observed
                ? "REPRODUCED: segmentation changes valid text and malformed bytes are accepted."
                : "NOT REPRODUCED: original starter symptom absent.");
            return observed ? 0 : 1;
        }

        bool verified = results.All(r => r.Passed);
        Console.WriteLine(verified
            ? "VERIFIED: valid text is chunk-independent and malformed input is rejected."
            : "FAILED: decoding contract is not satisfied.");
        return verified ? 0 : 1;
    }

    private static Probe Valid(string name, string expected, ReadOnlyMemory<byte>[] fragments)
    {
        try
        {
            string actual = ChunkedNoteDecoder.Decode(fragments);
            return new Probe(name, actual == expected, false, actual.Count(c => c == '\uFFFD'), fragments.Length);
        }
        catch (DecoderFallbackException)
        {
            return new Probe(name, false, true, 0, fragments.Length);
        }
    }

    private static Probe Invalid(string name, ReadOnlyMemory<byte>[] fragments)
    {
        try
        {
            string actual = ChunkedNoteDecoder.Decode(fragments);
            return new Probe(name, false, false, actual.Count(c => c == '\uFFFD'), fragments.Length);
        }
        catch (DecoderFallbackException)
        {
            return new Probe(name, true, true, 0, fragments.Length);
        }
    }

    private static ReadOnlyMemory<byte>[] Split(byte[] bytes, params int[] sizes)
    {
        var chunks = new List<ReadOnlyMemory<byte>>();
        int offset = 0;
        foreach (int size in sizes)
        {
            if (offset == bytes.Length) break;
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(sizes));
            int length = Math.Min(size, bytes.Length - offset);
            chunks.Add(bytes.AsMemory(offset, length));
            offset += length;
        }

        if (offset < bytes.Length)
            chunks.Add(bytes.AsMemory(offset));

        return chunks.ToArray();
    }
}
