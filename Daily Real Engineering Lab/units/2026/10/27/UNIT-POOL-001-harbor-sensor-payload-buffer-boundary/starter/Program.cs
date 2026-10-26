using System.Buffers;
using System.Text;
using TelemetryRelay;

return args.FirstOrDefault() switch
{
    "--reproduce" => Reproduce(),
    "--verify" => Verify(),
    _ => Demo()
};

static int Demo()
{
    byte[] input = Encoding.UTF8.GetBytes("ALERT");
    string encoded = new PacketPayloadEncoder(new FixedSizePool())
        .Encode(new MemoryStream(input, writable: false), input.Length);
    Console.WriteLine($"Input bytes={input.Length}; outbound bytes={Convert.FromBase64String(encoded).Length}");
    return 0;
}

static int Reproduce()
{
    byte[] input = Encoding.UTF8.GetBytes("ALERT");
    var pool = new FixedSizePool();
    byte[] actual = Convert.FromBase64String(new PacketPayloadEncoder(pool)
        .Encode(new MemoryStream(input, writable: false), input.Length));
    Console.WriteLine($"Declared={input.Length}; decoded={actual.Length}; returned={pool.ReturnCount}");
    if (actual.AsSpan().SequenceEqual(input))
    {
        Console.Error.WriteLine("Expected starter symptom not reproduced.");
        return 1;
    }
    Console.WriteLine("EXPECTED STARTER FAILURE: outbound payload differs from input.");
    return 0;
}

static int Verify()
{
    byte[][] samples =
    [
        [],
        Encoding.UTF8.GetBytes("ALERT"),
        [0x00, 0xFF, 0x00, 0x7F],
        Enumerable.Range(0, 31).Select(x => (byte)x).ToArray(),
        Enumerable.Range(0, 32).Select(x => (byte)x).ToArray()
    ];
    int failures = 0;
    for (int i = 0; i < samples.Length; i++)
    {
        byte[] expected = samples[i];
        var pool = new FixedSizePool();
        byte[] actual = Convert.FromBase64String(new PacketPayloadEncoder(pool)
            .Encode(new MemoryStream(expected, writable: false), expected.Length));
        bool equal = actual.AsSpan().SequenceEqual(expected);
        bool returnedOnce = pool.ReturnCount == 1;
        Console.WriteLine($"Case {i}: expected={expected.Length} actual={actual.Length} equal={equal} returnedOnce={returnedOnce}");
        if (!equal || !returnedOnce) failures++;
    }
    if (failures > 0)
    {
        Console.Error.WriteLine($"VERIFY FAILED: {failures} cases.");
        return 1;
    }
    Console.WriteLine("VERIFY PASSED: all cases.");
    return 0;
}

sealed class FixedSizePool : ArrayPool<byte>
{
    public int ReturnCount { get; private set; }
    public override byte[] Rent(int minimumLength)
    {
        if (minimumLength > 32)
            throw new ArgumentOutOfRangeException(nameof(minimumLength));
        var rented = new byte[32];
        Array.Fill(rented, (byte)0xD7);
        return rented;
    }
    public override void Return(byte[] array, bool clearArray = false)
    {
        if (array.Length != 32) throw new InvalidOperationException("Unexpected rental");
        if (clearArray) Array.Clear(array);
        ReturnCount++;
    }
}
