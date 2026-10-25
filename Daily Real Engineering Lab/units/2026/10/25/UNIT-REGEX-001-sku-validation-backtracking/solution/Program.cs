using System;
using System.Text.RegularExpressions;

namespace RegexLab;

internal static class Program
{
    private static int Main(string[] args)
    {
        var mode = args.Length == 1 ? args[0] : "--demo";
        return mode switch
        {
            "--demo" => Demo(),
            "--reproduce" => Reproduce(),
            "--verify" => Verify(),
            _ => Usage()
        };
    }

    private static int Usage()
    {
        Console.Error.WriteLine("Usage: --demo | --reproduce | --verify");
        return 2;
    }

    private static int Demo()
    {
        foreach (var value in new[] { "PART2026", "PART-2026", new string('A', 512) + "!" })
        {
            try
            {
                Console.WriteLine($"SKU length={value.Length}; accepted={SkuValidator.IsValid(value)}");
            }
            catch (RegexMatchTimeoutException)
            {
                Console.WriteLine($"SKU length={value.Length}; validation timed out");
            }
        }

        return 0;
    }

    private static int Reproduce()
    {
        if (!SkuValidator.IsValid("PART2026") || SkuValidator.IsValid("PART-2026"))
        {
            Console.Error.WriteLine("Baseline contract failed; investigate the validator first.");
            return 2;
        }

        try
        {
            bool accepted = SkuValidator.IsValid(new string('A', 512) + "!");
            Console.Error.WriteLine($"NOT REPRODUCED: long invalid SKU returned {accepted} without timeout.");
            return 1;
        }
        catch (RegexMatchTimeoutException ex)
        {
            Console.WriteLine($"REPRODUCED: invalid SKU triggered {ex.GetType().Name}.");
            return 0;
        }
    }

    private static int Verify()
    {
        var cases = new (string? Input, bool Expected, string Label)[]
        {
            ("A", true, "single valid character"),
            ("PART2026", true, "normal valid SKU"),
            ("Z9", true, "short alphanumeric SKU"),
            ("PART-2026", false, "hyphen is disallowed"),
            ("part2026", false, "lowercase is disallowed"),
            ("", false, "empty SKU"),
            (null, false, "null SKU"),
            ("PART 2026", false, "space is disallowed"),
            (new string('A', 512) + "!", false, "adversarial near-match"),
            (new string('A', 2049), false, "input above length policy")
        };

        int failures = 0;
        foreach (var test in cases)
        {
            try
            {
                bool actual = SkuValidator.IsValid(test.Input);
                if (actual != test.Expected)
                {
                    Console.Error.WriteLine($"FAIL: {test.Label}; expected={test.Expected}, actual={actual}");
                    failures++;
                }
                else
                {
                    Console.WriteLine($"PASS: {test.Label}");
                }
            }
            catch (RegexMatchTimeoutException)
            {
                Console.Error.WriteLine($"FAIL: {test.Label}; validation timed out");
                failures++;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"FAIL: {test.Label}; unexpected {ex.GetType().Name}: {ex.Message}");
                failures++;
            }
        }

        Console.WriteLine($"Verification: {cases.Length - failures}/{cases.Length} passed.");
        return failures == 0 ? 0 : 1;
    }
}
