using System;
using PlatformFeed;

namespace PlatformFeed;

internal static class Program
{
    private static int _failures;

    private static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "--demo" : args[0];
        if (mode == "--demo")
        {
            foreach (string raw in new[]
            {
                "SH-210|P4||North Terminal|BOARDING",
                "SH-211|P2|Gate C|Harbor Museum|DELAYED"
            })
            {
                Departure row = DepartureParser.Parse(raw);
                Console.WriteLine(
                    $"service={row.ServiceId}; platform={row.Platform}; note={row.Note}; destination={row.Destination}; status={row.Status}");
            }
            return 0;
        }

        if (mode != "--check")
        {
            Console.Error.WriteLine("Usage: --demo or --check");
            return 2;
        }

        Check("optional_note_empty_preserves_all_columns", () =>
        {
            Departure row = DepartureParser.Parse("SH-210|P4||North Terminal|BOARDING");
            return row.ServiceId == "SH-210" && row.Platform == "P4" &&
                   row.Note == "" && row.Destination == "North Terminal" &&
                   row.Status == "BOARDING";
        });

        Check("populated_note_preserves_all_columns", () =>
        {
            Departure row = DepartureParser.Parse("SH-211|P2|Gate C|Harbor Museum|DELAYED");
            return row.Note == "Gate C" && row.Destination == "Harbor Museum" &&
                   row.Status == "DELAYED";
        });

        Check("another_empty_note_preserves_status", () =>
        {
            Departure row = DepartureParser.Parse("SH-212|P1||West Park|ON_TIME");
            return row.Note == "" && row.Destination == "West Park" &&
                   row.Status == "ON_TIME";
        });

        Reject("missing_field_rejected", "SH-213|P1|West Park|ON_TIME");
        Reject("empty_required_destination_rejected", "SH-214|P1|Gate A||ON_TIME");
        Reject("extra_field_rejected", "SH-215|P1||West Park|ON_TIME|unexpected");
        Reject("empty_required_status_rejected", "SH-216|P1|Gate A|West Park|");

        Console.WriteLine($"SUMMARY total=7 failed={_failures}");
        return _failures == 0 ? 0 : 1;
    }

    private static void Check(string name, Func<bool> assertion)
    {
        try
        {
            if (assertion())
                Console.WriteLine($"CONTRACT_PASS {name}");
            else
            {
                _failures++;
                Console.WriteLine($"CONTRACT_FAIL {name}: unexpected parsed value");
            }
        }
        catch (Exception ex)
        {
            _failures++;
            Console.WriteLine($"CONTRACT_FAIL {name}: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void Reject(string name, string input)
    {
        Check(name, () =>
        {
            try
            {
                DepartureParser.Parse(input);
                return false;
            }
            catch (FormatException)
            {
                return true;
            }
        });
    }
}
