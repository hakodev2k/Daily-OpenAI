using GateFrameLab;

return args.Length == 1 ? args[0] switch
{
    "--inspect" => Inspect(),
    "--reproduce" => Reproduce(),
    "--verify" => Verify(),
    _ => Usage()
} : Usage();

static int Usage()
{
    Console.Error.WriteLine("Usage: dotnet run --project starter/GateFrameLab.csproj -- --inspect|--reproduce|--verify");
    return 2;
}

static byte[] FrameA() => [0xA5, 0x01, 0x00, 0x2A, 0x03, 0xE8, 0x00, 0x00];
static byte[] FrameB() => [0xA5, 0x01, 0x00, 0x07, 0x01, 0x04, 0x00, 0x00];
static byte[] FrameC() => [0xA5, 0x01, 0x00, 0x01, 0xFF, 0xFF, 0x00, 0x00];

static int Inspect()
{
    foreach (var (name, frame) in new[] { ("A", FrameA()), ("B", FrameB()), ("C", FrameC()) })
    {
        var result = GateFrameDecoder.Decode(frame);
        Console.WriteLine($"Frame {name} [{Convert.ToHexString(frame)}]: GateId={result.GateId}, EntryCount={result.EntryCount}");
    }
    return 0;
}

static int Reproduce()
{
    try
    {
        var a = GateFrameDecoder.Decode(FrameA());
        var b = GateFrameDecoder.Decode(FrameB());
        Console.WriteLine($"A={a}, vendor=GateSample {{ GateId = 42, EntryCount = 1000 }}");
        Console.WriteLine($"B={b}, vendor=GateSample {{ GateId = 7, EntryCount = 260 }}");
        if (a == new GateSample(10752, 59395) && b == new GateSample(1792, 1025))
        {
            Console.WriteLine("REPRODUCED: gateway reports the original mismatch.");
            return 0;
        }
        Console.Error.WriteLine("NOT REPRODUCED: original mismatch is absent.");
        return 1;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("UNEXPECTED FAILURE: " + ex);
        return 2;
    }
}

static int Verify()
{
    try
    {
        Check(FrameA(), new GateSample(42, 1000), "A");
        Check(FrameB(), new GateSample(7, 260), "B");
        Check(FrameC(), new GateSample(1, 65535), "C");
        ExpectInvalid([0xA5, 0x01, 0x00], "short frame");
        ExpectInvalid([0x00, 0x01, 0x00, 0x2A, 0x03, 0xE8, 0x00, 0x00], "invalid signature");
        ExpectInvalid([0xA5, 0x02, 0x00, 0x2A, 0x03, 0xE8, 0x00, 0x00], "unsupported version");
        ExpectInvalid([0xA5, 0x01, 0x00, 0x2A, 0x03, 0xE8, 0x00, 0x01], "reserved field");
        Console.WriteLine("PASS: valid fixtures match vendor; malformed frames rejected.");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("FAIL: " + ex.Message);
        return 1;
    }
}

static void Check(byte[] frame, GateSample expected, string name)
{
    var actual = GateFrameDecoder.Decode(frame);
    if (actual != expected)
        throw new InvalidOperationException($"Frame {name}: expected {expected}, got {actual}.");
}

static void ExpectInvalid(byte[] frame, string name)
{
    try { _ = GateFrameDecoder.Decode(frame); }
    catch (ArgumentException) { return; }
    catch (FormatException) { return; }
    throw new InvalidOperationException($"Frame {name}: invalid frame accepted.");
}
