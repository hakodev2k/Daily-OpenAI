namespace ReminderScheduler;

public readonly record struct Reminder(DateOnly LocalDate, DateTime ScheduledUtc);

internal static class Program
{
    private static int Main(string[] args)
    {
        var mode = args.Length == 0 ? "demo" : args[0].TrimStart('-').ToLowerInvariant();

        // These are independent contractual UTC instants for 09:00 in New York.
        // They are not calculated by ScheduleBuilder or by its conversion method.
        Reminder[] expected =
        [
            new(new DateOnly(2026, 3, 7),  new DateTime(2026, 3, 7, 14, 0, 0, DateTimeKind.Utc)),
            new(new DateOnly(2026, 3, 8),  new DateTime(2026, 3, 8, 13, 0, 0, DateTimeKind.Utc)),
            new(new DateOnly(2026, 3, 9),  new DateTime(2026, 3, 9, 13, 0, 0, DateTimeKind.Utc)),
            new(new DateOnly(2026, 10, 31), new DateTime(2026, 10, 31, 13, 0, 0, DateTimeKind.Utc)),
            new(new DateOnly(2026, 11, 1), new DateTime(2026, 11, 1, 14, 0, 0, DateTimeKind.Utc)),
            new(new DateOnly(2026, 11, 2), new DateTime(2026, 11, 2, 14, 0, 0, DateTimeKind.Utc))
        ];

        var actual = ScheduleBuilder.Build(new DateOnly(2026, 3, 7), 3)
            .Concat(ScheduleBuilder.Build(new DateOnly(2026, 10, 31), 3))
            .ToArray();

        var allCorrect = actual.SequenceEqual(expected);
        var anchorsCorrect = actual.Length == expected.Length &&
                             actual[0] == expected[0] && actual[3] == expected[3];

        Console.WriteLine("Contract: 09:00 America/New_York per local calendar date");
        Console.WriteLine("Local date  | Actual UTC             | Expected UTC           | Match");
        for (var i = 0; i < Math.Max(actual.Length, expected.Length); i++)
        {
            if (i >= actual.Length || i >= expected.Length)
            {
                Console.WriteLine($"row {i}: missing or extra reminder");
                continue;
            }

            var a = actual[i];
            var e = expected[i];
            Console.WriteLine($"{a.LocalDate:yyyy-MM-dd}  | {a.ScheduledUtc:yyyy-MM-dd HH:mm}Z | {e.ScheduledUtc:yyyy-MM-dd HH:mm}Z | {a == e}");
        }

        if (mode == "reproduce")
        {
            var reproduced = anchorsCorrect && !allCorrect;
            Console.WriteLine(reproduced ? "REPRODUCED: anchor days pass, later days drift." :
                "NOT REPRODUCED: expected starter symptom was not observed.");
            return reproduced ? 0 : 1;
        }

        if (mode == "verify")
        {
            Console.WriteLine(allCorrect ? "PASS: all six independent contract cases match." :
                "FAIL: one or more UTC instants violate the 09:00 local-time contract.");
            return allCorrect ? 0 : 1;
        }

        return mode == "demo" ? 0 : 2;
    }
}
