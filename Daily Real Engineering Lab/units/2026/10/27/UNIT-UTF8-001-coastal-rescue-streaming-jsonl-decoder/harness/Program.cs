using System.Text;
using System.Text.Json;
using Lab;
return Harness.Run(args);
internal static class Harness
{
    const string A = "{\"Id\":\"SEA-41\",\"Message\":\"Sóng lớn ở đảo 🛟\"}";
    const string B = "{\"Id\":\"SEA-42\",\"Message\":\"Tàu cập bến an toàn\"}";
    const string Payload = A + "\n" + B + "\n";
    public static int Run(string[] args)
    {
        try
        {
            string mode = args.Length == 0 ? "--show" : args[0];
            return mode switch
            {
                "--show" => Show(),
                "--reproduce" => Reproduce(),
                "--verify" => Verify(),
                _ => 2
            };
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 2; }
    }
    static int Show() { Print(Utf8LineReader.ReadLines(Split())); return 0; }
    static int Reproduce()
    {
        var lines = Utf8LineReader.ReadLines(Split());
        Print(lines);
        if (Valid(lines)) { Console.Error.WriteLine("Baseline unexpectedly passed."); return 1; }
        Console.WriteLine("REPRODUCED: wire text differs from decoded records.");
        return 0;
    }
    static int Verify()
    {
        var tests = new (string Name, byte[][] Frames)[]
        {
            ("single read", new[] { Encoding.UTF8.GetBytes(Payload) }),
            ("split inside UTF-8 sequences", Split()),
            ("one byte per read", Encoding.UTF8.GetBytes(Payload).Select(b => new[] { b }).ToArray()),
            ("split near end", Slice(Encoding.UTF8.GetBytes(Payload), Encoding.UTF8.GetByteCount(Payload) - 2))
        };
        foreach (var test in tests)
        {
            var lines = Utf8LineReader.ReadLines(test.Frames);
            if (!Valid(lines)) { Console.Error.WriteLine("FAIL: " + test.Name); Print(lines); return 1; }
            Console.WriteLine("PASS: " + test.Name);
        }
        try
        {
            Utf8LineReader.ReadLines(new[] { new byte[] { 0x7B, 0x22, 0x78, 0x22, 0x3A, 0x22, 0xF0, 0x9F } });
            Console.Error.WriteLine("FAIL: truncated UTF-8 accepted."); return 1;
        }
        catch (DecoderFallbackException) { Console.WriteLine("PASS: truncated UTF-8 rejected."); }
        Console.WriteLine("VERIFY PASSED."); return 0;
    }
    static bool Valid(IReadOnlyList<string> lines)
    {
        if (lines.Count != 2 || lines[0] != A || lines[1] != B) return false;
        try
        {
            foreach (string line in lines)
            {
                using var doc = JsonDocument.Parse(line);
                if (doc.RootElement.GetProperty("Id").GetString() is null ||
                    doc.RootElement.GetProperty("Message").GetString() is null) return false;
            }
            return true;
        }
        catch (JsonException) { return false; }
    }
    static void Print(IReadOnlyList<string> lines)
    {
        Console.WriteLine("Expected: " + A);
        Console.WriteLine("Actual:   " + (lines.Count > 0 ? lines[0] : "<missing>"));
        Console.WriteLine("Records: " + lines.Count);
        Console.WriteLine("Replacement chars: " + lines.Sum(s => s.Count(c => c == '\uFFFD')));
    }
    static byte[][] Split()
    {
        byte[] data = Encoding.UTF8.GetBytes(Payload);
        int accent = Find(data, Encoding.UTF8.GetBytes("ó"));
        int emoji = Find(data, Encoding.UTF8.GetBytes("🛟"));
        return Slice(data, accent + 1, emoji + 2, data.Length - 2);
    }
    static int Find(byte[] data, byte[] pattern)
    {
        for (int i = 0; i <= data.Length - pattern.Length; i++)
            if (data.AsSpan(i, pattern.Length).SequenceEqual(pattern)) return i;
        throw new InvalidOperationException("Missing fixture marker.");
    }
    static byte[][] Slice(byte[] data, params int[] cuts)
    {
        var frames = new List<byte[]>();
        int start = 0;
        foreach (int end in cuts.Append(data.Length))
        {
            if (end <= start || end > data.Length) throw new InvalidOperationException("Invalid split.");
            frames.Add(data.AsSpan(start, end - start).ToArray());
            start = end;
        }
        return frames.ToArray();
    }
}
