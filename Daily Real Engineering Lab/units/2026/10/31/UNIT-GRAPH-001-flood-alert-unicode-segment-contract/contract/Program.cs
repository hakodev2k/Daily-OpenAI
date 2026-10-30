using System.Globalization;
using System.Text;

var mode = args.Length == 1 ? args[0].ToLowerInvariant() : "demo";
if (mode is not ("demo" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [demo|reproduce|verify]");
    return 2;
}

var cases = new[]
{
    new Case("ASCII baseline", "ASCII", 5),
    new Case("Combining accent boundary", "ABCDe\u0301XYZ", 5),
    new Case("Surrogate pair boundary", "ABCD😀XYZ", 5),
    new Case("UTF-8 budget vs UTF-16 length", "AAAAé", 5),
    new Case("Single grapheme exceeds budget", "é", 1, true),
    new Case("Empty payload", "", 5)
};
var strictUtf8 = new UTF8Encoding(false, true);
var failures = 0;

foreach (var test in cases)
{
    try
    {
        var parts = MessageSplitter.Split(test.Text, test.MaxBytes);
        if (test.ExpectRejection)
        {
            Fail("oversized text element was accepted");
            continue;
        }

        var errors = new List<string>();
        if (!string.Equals(string.Concat(parts), test.Text, StringComparison.Ordinal))
            errors.Add("round-trip mismatch");

        var validBoundaries = new HashSet<int>(
            StringInfo.ParseCombiningCharacters(test.Text)) { test.Text.Length };
        var offset = 0;
        foreach (var part in parts)
        {
            if (part.Length == 0)
                errors.Add("empty segment");
            try
            {
                var byteCount = strictUtf8.GetByteCount(part);
                if (byteCount > test.MaxBytes)
                    errors.Add($"segment requires {byteCount} bytes, budget {test.MaxBytes}");
            }
            catch (EncoderFallbackException)
            {
                errors.Add("segment contains an invalid UTF-16 surrogate sequence");
            }

            offset += part.Length;
            if (!validBoundaries.Contains(offset))
                errors.Add($"split inside a text element at UTF-16 index {offset}");
        }

        if (errors.Count == 0)
            Console.WriteLine($"PASS {test.Name}: {parts.Count} segment(s)");
        else
            Fail(string.Join("; ", errors));
    }
    catch (ArgumentException ex) when (test.ExpectRejection)
    {
        Console.WriteLine($"PASS {test.Name}: rejected ({ex.GetType().Name})");
    }
    catch (Exception ex)
    {
        Fail($"unexpected {ex.GetType().Name}: {ex.Message}");
    }

    void Fail(string details)
    {
        failures++;
        Console.WriteLine($"FAIL {test.Name}: {details}");
    }
}

Console.WriteLine($"Contract failures: {failures}");
if (mode == "reproduce")
{
    if (failures >= 3)
    {
        Console.WriteLine("Reproduction confirmed (expected starter failures).");
        return 0;
    }
    Console.Error.WriteLine("Expected starter failures were not reproduced.");
    return 1;
}
if (mode == "verify")
{
    if (failures == 0)
    {
        Console.WriteLine("Learner contract verified.");
        return 0;
    }
    Console.Error.WriteLine("Learner contract still fails.");
    return 1;
}
return 0;

internal sealed record Case(string Name, string Text, int MaxBytes,
    bool ExpectRejection = false);
