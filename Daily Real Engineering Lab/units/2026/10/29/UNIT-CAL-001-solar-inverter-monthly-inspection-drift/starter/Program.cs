using System;
using System.Collections.Generic;
using System.Linq;

return LabRunner.Run(args.Length == 0 ? "run" : args[0]);

public static class InspectionSchedule
{
    // Business contract: one occurrence per calendar month, with the desired
    // original day or the last day of a shorter month.
    public static IReadOnlyList<DateOnly> Build(DateOnly firstDue, int count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        var dates = new List<DateOnly>(count);
        var due = firstDue;
        for (var i = 0; i < count; i++)
        {
            dates.Add(due);
            due = due.AddMonths(1);
        }
        return dates;
    }
}

internal static class LabRunner
{
    private sealed record Fixture(string Name, DateOnly Start, DateOnly[] Expected);
    private static readonly Fixture[] Cases =
    [
        new("end-of-month-2026", new DateOnly(2026, 1, 31),
            [new(2026, 1, 31), new(2026, 2, 28), new(2026, 3, 31),
             new(2026, 4, 30), new(2026, 5, 31)]),
        new("leap-year-2028", new DateOnly(2028, 1, 31),
            [new(2028, 1, 31), new(2028, 2, 29), new(2028, 3, 31),
             new(2028, 4, 30)]),
        new("day-30", new DateOnly(2026, 1, 30),
            [new(2026, 1, 30), new(2026, 2, 28), new(2026, 3, 30),
             new(2026, 4, 30)]),
        new("mid-month-control", new DateOnly(2026, 1, 15),
            [new(2026, 1, 15), new(2026, 2, 15), new(2026, 3, 15),
             new(2026, 4, 15)]),
        new("year-boundary", new DateOnly(2026, 11, 30),
            [new(2026, 11, 30), new(2026, 12, 30), new(2027, 1, 30),
             new(2027, 2, 28), new(2027, 3, 30)])
    ];

    public static int Run(string mode)
    {
        if (mode is not ("run" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: run | reproduce | verify");
            return 2;
        }
        var outcomes = Cases.Select(Check).ToArray();
        var zero = InspectionSchedule.Build(new DateOnly(2026, 1, 31), 0).Count == 0;
        var negativeThrows = false;
        try { InspectionSchedule.Build(new DateOnly(2026, 1, 31), -1); }
        catch (ArgumentOutOfRangeException) { negativeThrows = true; }
        Console.WriteLine($"count=0 empty: {zero}; negative count rejected: {negativeThrows}");
        if (mode == "run") return 0;
        if (mode == "reproduce")
        {
            var baseline = !outcomes[0] && outcomes[3] && zero && negativeThrows;
            Console.WriteLine(baseline ? "REPRODUCED: end-of-month drift." : "NOT REPRODUCED.");
            return baseline ? 0 : 1;
        }
        var pass = outcomes.All(x => x) && zero && negativeThrows;
        Console.WriteLine(pass ? "PASS: all recurrence contracts." : "FAIL: recurrence contract.");
        return pass ? 0 : 1;
    }

    private static bool Check(Fixture fixture)
    {
        var observed = InspectionSchedule.Build(fixture.Start, fixture.Expected.Length);
        var pass = observed.SequenceEqual(fixture.Expected);
        Console.WriteLine($"{fixture.Name,-20} {(pass ? "PASS" : "FAIL")}");
        Console.WriteLine($"  expected: {string.Join(", ", fixture.Expected)}");
        Console.WriteLine($"  actual:   {string.Join(", ", observed)}");
        return pass;
    }
}
