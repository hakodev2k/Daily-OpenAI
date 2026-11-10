using System.Buffers.Binary;

static byte[] BuildFrame(ReadOnlySpan<byte> payload)
{
    using var stream = new MemoryStream(capacity: 256);
    Span<byte> header = stackalloc byte[2];
    BinaryPrimitives.WriteUInt16BigEndian(header, checked((ushort)payload.Length));
    stream.Write(header);
    stream.Write(payload);
    return stream.GetBuffer();
}

static bool ReceiverAccepts(byte[] frame, byte[] expectedPayload)
{
    if (frame.Length < 2) return false;
    var length = BinaryPrimitives.ReadUInt16BigEndian(frame.AsSpan(0, 2));
    return length == expectedPayload.Length &&
           frame.Length == length + 2 &&
           frame.AsSpan(2).SequenceEqual(expectedPayload);
}

var cases = new[] { 0, 5, 200 };
bool allAccepted = true;
foreach (int size in cases)
{
    var payload = Enumerable.Range(1, size).Select(i => (byte)(i % 251)).ToArray();
    byte[] frame = BuildFrame(payload);
    bool accepted = ReceiverAccepts(frame, payload);
    Console.WriteLine($"payloadLength={size} frameLength={size + 2} wireLength={frame.Length} accepted={accepted} tailHex={Convert.ToHexString(frame.AsSpan(Math.Max(0, frame.Length - 8)))}");
    allAccepted &= accepted;
}
if (args.Contains("reproduce", StringComparer.OrdinalIgnoreCase))
    return allAccepted ? 1 : 0; // 0 means symptom observed
if (args.Contains("verify", StringComparer.OrdinalIgnoreCase))
    return allAccepted ? 0 : 1; // 0 means all contracts passed
return 0;
