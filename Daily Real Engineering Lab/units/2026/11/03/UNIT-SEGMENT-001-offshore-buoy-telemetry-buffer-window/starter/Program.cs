namespace TelemetryLab;

internal static class Program
{
    private sealed record Fixture(string Name, byte[] Buffer, int Offset, int Count, byte[] Expected);

    private static readonly Fixture[] Cases =
    {
        new("full-buffer", new byte[] { 0x10, 0x20, 0x30 }, 0, 3,
            new byte[] { 0x00, 0x03, 0x10, 0x20, 0x30 }),
        new("offset-two", new byte[] { 0xEE, 0xDD, 0x10, 0x20, 0x30, 0xFA }, 2, 3,
            new byte[] { 0x00, 0x03, 0x10, 0x20, 0x30 }),
        new("trailing-window", new byte[] { 0xAA, 0x10, 0x20, 0x30 }, 1, 3,
            new byte[] { 0x00, 0x03, 0x10, 0x20, 0x30 }),
        new("empty-window", new byte[] { 0xCC, 0xDD }, 2, 0,
            new byte[] { 0x00, 0x00 })
    };

    private static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "inspect" : args[0].ToLowerInvariant();
        if (mode is not ("inspect" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: inspect | reproduce | verify");
            return 2;
        }

        int failures = 0;
        bool baselinePass = false;
        foreach (var fixture in Cases)
        {
            byte[] actual = TelemetryFrameWriter.Encode(
                new ArraySegment<byte>(fixture.Buffer, fixture.Offset, fixture.Count));
            bool passed = actual.AsSpan().SequenceEqual(fixture.Expected);
            if (fixture.Name == "full-buffer") baselinePass = passed;
            if (!passed) failures++;
            Console.WriteLine($"{fixture.Name}: {(passed ? "PASS" : "FAIL")}");
            Console.WriteLine($"  expected: {Convert.ToHexString(fixture.Expected)}");
            Console.WriteLine($"  actual:   {Convert.ToHexString(actual)}");
        }

        Console.WriteLine($"Summary: {Cases.Length - failures}/{Cases.Length} passed");
        if (mode == "inspect") return 0;
        if (mode == "reproduce")
        {
            bool reproduced = baselinePass && failures == 2;
            Console.WriteLine(reproduced ? "REPRODUCED: starter symptom" :
                "NOT REPRODUCED: starter differs from known baseline");
            return reproduced ? 0 : 1;
        }

        Console.WriteLine(failures == 0 ? "VERIFIED: all frame contracts hold" :
            "NOT VERIFIED: learner implementation violates frame contract");
        return failures == 0 ? 0 : 1;
    }
}
