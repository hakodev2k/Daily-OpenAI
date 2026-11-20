using System.Buffers.Binary;
using System.Text;

var messages = new[] { "SAFE-42", "Cứu trợ: nước sạch đến xã Bình An" };
using var stream = new MemoryStream();
foreach (var message in messages)
{
    var payload = Encoding.UTF8.GetBytes(message);
    Span<byte> header = stackalloc byte[4];
    BinaryPrimitives.WriteInt32LittleEndian(header, payload.Length);
    stream.Write(header);
    stream.Write(payload);
}
var bytes = stream.ToArray();
Console.WriteLine($"WIRE_BYTES={bytes.Length}");
try
{
    stream.Position = 0;
    var decoded = new List<string>();
    while (stream.Position < stream.Length)
    {
        Span<byte> header = stackalloc byte[4];
        stream.ReadExactly(header);
        var length = BinaryPrimitives.ReadInt32LittleEndian(header);
        Console.WriteLine($"DECLARED_LENGTH={length}");
        if (length < 0 || length > 4096) throw new InvalidDataException("Invalid frame length");
        var body = new byte[length];
        stream.ReadExactly(body);
        var text = new UTF8Encoding(false, true).GetString(body);
        decoded.Add(text);
    }
    var ok = decoded.SequenceEqual(messages);
    Console.WriteLine($"ROUNDTRIP_OK={ok}");
    if (!ok) Environment.ExitCode = 2;
}
catch (Exception ex)
{
    Console.WriteLine($"FRAME_ERROR={ex.GetType().Name}: {ex.Message}");
    Environment.ExitCode = 2;
}
