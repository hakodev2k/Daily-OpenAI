using System.Text;
using PreviewLeaseLab;

return args.FirstOrDefault()?.ToLowerInvariant() switch
{
    "reproduce" => Reproduce(),
    "verify" => Verify(),
    "run" or null => Run(),
    _ => Usage()
};

static int Usage()
{
    Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
    return 2;
}

static int Run()
{
    var service = new PreviewService(new RecyclingArrayPool());
    var a = service.Prepare(Encoding.UTF8.GetBytes("ROAD-SECTION-A"));
    Console.WriteLine($"A immediately: {Encoding.UTF8.GetString(a.Span)}");
    var b = service.Prepare(Encoding.UTF8.GetBytes("ROAD-SECTION-B"));
    Console.WriteLine($"A after B:    {Encoding.UTF8.GetString(a.Span)}");
    Console.WriteLine($"B:            {Encoding.UTF8.GetString(b.Span)}");
    return 0;
}

static int Reproduce()
{
    var service = new PreviewService(new RecyclingArrayPool());
    var a = service.Prepare(Encoding.UTF8.GetBytes("ROAD-SECTION-A"));
    string before = Encoding.UTF8.GetString(a.Span);
    var b = service.Prepare(Encoding.UTF8.GetBytes("ROAD-SECTION-B"));
    string after = Encoding.UTF8.GetString(a.Span);
    string second = Encoding.UTF8.GetString(b.Span);
    Console.WriteLine($"Before: {before}");
    Console.WriteLine($"After:  {after}");
    Console.WriteLine($"Second: {second}");
    bool symptom = before == "ROAD-SECTION-A"
        && after != "ROAD-SECTION-A"
        && second == "ROAD-SECTION-B";
    Console.WriteLine(symptom ? "REPRODUCED: previously prepared preview changed" : "NOT REPRODUCED");
    return symptom ? 0 : 1;
}

static int Verify()
{
    int failed = 0;
    void Check(string name, bool ok)
    {
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name}");
        if (!ok) failed++;
    }

    var service = new PreviewService(new RecyclingArrayPool());
    byte[] firstSource = Encoding.UTF8.GetBytes("ROAD-SECTION-A");
    ReadOnlyMemory<byte> first = service.Prepare(firstSource);
    Check("initial bytes", first.Span.SequenceEqual(Encoding.UTF8.GetBytes("ROAD-SECTION-A")));

    ReadOnlyMemory<byte> second = service.Prepare(Encoding.UTF8.GetBytes("ROAD-SECTION-B"));
    Check("first survives second request", first.Span.SequenceEqual(Encoding.UTF8.GetBytes("ROAD-SECTION-A")));
    Check("second payload correct", second.Span.SequenceEqual(Encoding.UTF8.GetBytes("ROAD-SECTION-B")));

    firstSource[0] = (byte)'X';
    Check("first independent of caller input", first.Span.SequenceEqual(Encoding.UTF8.GetBytes("ROAD-SECTION-A")));

    ReadOnlyMemory<byte> empty = service.Prepare(ReadOnlySpan<byte>.Empty);
    Check("empty preview", empty.IsEmpty);

    byte[] largeSource = Enumerable.Range(0, 130).Select(i => (byte)(i % 251)).ToArray();
    ReadOnlyMemory<byte> large = service.Prepare(largeSource);
    _ = service.Prepare(Encoding.UTF8.GetBytes("SMALL"));
    Check("large preview survives later request", large.Span.SequenceEqual(largeSource));
    Check("first remains stable after more requests", first.Span.SequenceEqual(Encoding.UTF8.GetBytes("ROAD-SECTION-A")));

    Console.WriteLine(failed == 0 ? "VERIFY PASSED" : $"VERIFY FAILED: {failed} checks");
    return failed == 0 ? 0 : 1;
}
