using System.Text;
using RadioGateway;

const string message = "Trạm Cứu hộ 🦉 - Bản Mây";
var payload = Encoding.UTF8.GetBytes(message);

static byte[][] Split(byte[] data, params int[] sizes)
{
    var chunks = new List<byte[]>();
    var offset = 0;
    foreach (var size in sizes)
    {
        if (size <= 0 || offset + size > data.Length)
            throw new ArgumentOutOfRangeException(nameof(sizes));
        chunks.Add(data.AsSpan(offset, size).ToArray());
        offset += size;
    }
    if (offset < data.Length) chunks.Add(data.AsSpan(offset).ToArray());
    return chunks.ToArray();
}
static void Check(string name, byte[][] chunks, string expected, List<string> errors)
{
    var actual = FrameReader.ReadMessage(chunks);
    var pass = actual == expected;
    Console.WriteLine($"{name}: {(pass ? "PASS" : "FAIL")} | chunks=[{string.Join(",", chunks.Select(x => x.Length))}] | output={actual}");
    if (!pass) errors.Add(name);
}
var mode = args.Length == 0 ? "run" : args[0];
var errors = new List<string>();
if (mode == "--run")
{
    Console.WriteLine($"payload(hex): {Convert.ToHexString(payload)}");
    Console.WriteLine(FrameReader.ReadMessage(Split(payload, 1)));
    return 0;
}
if (mode == "--reproduce")
{
    var splitAt = Array.IndexOf(payload, (byte)0xF0) + 1;
    if (splitAt <= 1) throw new InvalidOperationException("Fixture không có ký tự 4-byte.");
    Check("one-chunk", new[] { payload }, message, errors);
    Check("split-inside-codepoint", Split(payload, splitAt), message, errors);
    if (!errors.Contains("split-inside-codepoint")) return 1;
    Console.WriteLine("REPRODUCED: cùng payload, kết quả phụ thuộc chunk boundary.");
    return 0;
}
if (mode == "--verify")
{
    Check("one-chunk", new[] { payload }, message, errors);
    Check("byte-by-byte", payload.Select(x => new[] { x }).ToArray(), message, errors);
    var splitAt = Array.IndexOf(payload, (byte)0xF0) + 1;
    Check("split-inside-codepoint", Split(payload, splitAt), message, errors);
    Check("ascii", Split(Encoding.UTF8.GetBytes("Rescue-42"), 2, 3), "Rescue-42", errors);
    Check("empty", Array.Empty<byte[]>(), "", errors);
    Check("malformed-final", new[] { new byte[] { 0xE2 }, new byte[] { 0x82 } }, "\uFFFD", errors);
    Console.WriteLine(errors.Count == 0 ? "VERIFY PASS" : $"VERIFY FAIL: {string.Join(", ", errors)}");
    return errors.Count == 0 ? 0 : 1;
}
Console.Error.WriteLine("Use --run, --reproduce, or --verify");
return 2;

