using System.Buffers.Binary;

static class SensorDecoder
{
    // Payload: 2 bytes signed temperature in tenths of a degree, little-endian.
    public static short ReadTemperatureTenths(ArraySegment<byte> payload)
    {
        if (payload.Array is null || payload.Count < sizeof(short))
            throw new ArgumentException("Payload too short", nameof(payload));
        return BinaryPrimitives.ReadInt16LittleEndian(payload.Array.AsSpan(0, sizeof(short)));
    }
}

static class Program
{
    private static int Main(string[] args)
    {
        var fixtures = new (string Name, byte[] Buffer, int Offset, int Count, short Expected)[]
        {
            ("frame-A", new byte[] { 0x11, 0x22, 0xD7, 0x00, 0xFE }, 2, 2, 215),
            ("frame-B", new byte[] { 0x80, 0x00, 0x6A, 0xFF, 0xCC }, 2, 2, -150),
            ("frame-C", new byte[] { 0x7B, 0x00 }, 0, 2, 123),
            ("frame-D", new byte[] { 0x00, 0x00, 0x55, 0x34, 0x12, 0x99 }, 3, 2, 4660),
        };
        var failed = 0;
        foreach (var f in fixtures)
        {
            var actual = SensorDecoder.ReadTemperatureTenths(new ArraySegment<byte>(f.Buffer, f.Offset, f.Count));
            var ok = actual == f.Expected;
            Console.WriteLine($"{f.Name}: expected={f.Expected}, actual={actual}, pass={ok}");
            if (!ok) failed++;
        }
        try
        {
            SensorDecoder.ReadTemperatureTenths(new ArraySegment<byte>(new byte[] { 1, 2, 3 }, 2, 1));
            Console.WriteLine("short-window: expected ArgumentException, got success");
            failed++;
        }
        catch (ArgumentException) { Console.WriteLine("short-window: rejected"); }
        Console.WriteLine($"FAILED={failed}");
        return failed == 0 ? 0 : 1;
    }
}
