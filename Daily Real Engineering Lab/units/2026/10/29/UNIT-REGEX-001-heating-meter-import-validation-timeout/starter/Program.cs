using System.Text.RegularExpressions;

namespace MeterCodeLab;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run -- [reproduce|verify]");
            return 64;
        }

        var cases = new (string Name, string Value, bool Expected)[]
        {
            ("ordinary-valid", "HEAT2026A7", true),
            ("long-valid", new string('A', 64), true),
            ("hyphen-invalid", "HEAT-2026", false),
            ("empty-invalid", "", false),
            ("long-invalid", new string('A', 48) + "!", false)
        };

        var failures = 0;
        var timeouts = 0;
        foreach (var item in cases)
        {
            try
            {
                var actual = MeterCodeValidator.IsValid(item.Value);
                if (actual == item.Expected)
                {
                    Console.WriteLine($"PASS {item.Name}: accepted={actual}");
                }
                else
                {
                    failures++;
                    Console.WriteLine($"FAIL {item.Name}: expected={item.Expected}, actual={actual}");
                }
            }
            catch (RegexMatchTimeoutException)
            {
                failures++;
                timeouts++;
                Console.WriteLine($"FAIL {item.Name}: RegexMatchTimeoutException");
            }
        }

        if (args[0] == "reproduce")
        {
            if (failures == 1 && timeouts == 1)
            {
                Console.WriteLine("EXPECTED_FAILURE_REPRODUCED");
                return 0;
            }

            Console.Error.WriteLine($"REPRODUCTION_NOT_CONFIRMED failures={failures} timeouts={timeouts}");
            return 1;
        }

        if (failures == 0)
        {
            Console.WriteLine("VERIFICATION_PASSED");
            return 0;
        }

        Console.Error.WriteLine($"VERIFICATION_FAILED failures={failures} timeouts={timeouts}");
        return 1;
    }
}
