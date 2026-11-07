namespace FieldOpsLab;

internal static class Program
{
    private static readonly WorkEvent[] DemoEvents =
    [
        new("TECH-17", true),
        new("TECH-42", false),
        new("TECH-17", false),
        new("TECH-17", true)
    ];

    private static int Main(string[] args)
    {
        var mode = args.Length == 0 ? "run" : args[0].ToLowerInvariant();
        return mode switch
        {
            "run" => Run(),
            "reproduce" => Reproduce(),
            "verify" => Verify(),
            _ => Usage()
        };
    }

    private static int Usage()
    {
        Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
        return 2;
    }

    private static DailyRollup Ingest(IEnumerable<WorkEvent> events)
    {
        var rollup = new DailyRollup();
        foreach (var entry in events)
        {
            rollup.Add(entry);
            Console.WriteLine($"accepted technician={entry.TechnicianId} succeeded={entry.Succeeded}");
        }
        return rollup;
    }

    private static int Run()
    {
        var rollup = Ingest(DemoEvents);
        foreach (var id in new[] { "TECH-17", "TECH-42" })
        {
            var summary = rollup.Read(id);
            Console.WriteLine($"dashboard technician={id} completed={summary.Completed} failed={summary.Failed} total={summary.Total}");
        }
        return 0;
    }

    private static int Reproduce()
    {
        var rollup = Ingest(DemoEvents);
        var a = rollup.Read("TECH-17");
        var b = rollup.Read("TECH-42");
        Console.WriteLine($"observed TECH-17={a.Completed}/{a.Failed}/{a.Total}; TECH-42={b.Completed}/{b.Failed}/{b.Total}");
        var reproduced = a.Total == 0 && b.Total == 0 && DemoEvents.Length == 4;
        Console.WriteLine(reproduced ? "REPRODUCED: four accepted events, both dashboard totals are zero." :
            "NOT REPRODUCED: the original zero-total symptom is absent.");
        return reproduced ? 0 : 1;
    }

    private static int Verify()
    {
        var failures = 0;
        var rollup = Ingest(DemoEvents);
        var a = rollup.Read("TECH-17");
        var b = rollup.Read("TECH-42");
        Check("TECH-17 aggregation", a.Completed == 2 && a.Failed == 1 && a.Total == 3, ref failures);
        Check("TECH-42 isolation", b.Completed == 0 && b.Failed == 1 && b.Total == 1, ref failures);
        var missing = rollup.Read("TECH-99");
        Check("unknown technician is empty", missing.Total == 0, ref failures);

        var snapshot = rollup.Read("TECH-17");
        snapshot.Record(false);
        var persisted = rollup.Read("TECH-17");
        Check("read snapshot cannot mutate stored totals",
            persisted.Completed == 2 && persisted.Failed == 1, ref failures);

        try
        {
            rollup.Add(new WorkEvent("  ", true));
            Check("reject empty technician", false, ref failures);
        }
        catch (ArgumentException)
        {
            Check("reject empty technician", true, ref failures);
        }

        rollup.Add(new WorkEvent("TECH-42", true));
        var updated = rollup.Read("TECH-42");
        Check("increment existing technician without overwriting earlier totals",
            updated.Completed == 1 && updated.Failed == 1 && updated.Total == 2, ref failures);
        Check("other technician unchanged", rollup.Read("TECH-17").Total == 3, ref failures);

        Console.WriteLine(failures == 0 ? "VERIFIED: all behavioral checks passed." :
            $"NOT VERIFIED: {failures} behavioral checks failed.");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(string name, bool passed, ref int failures)
    {
        Console.WriteLine($"{(passed ? "PASS" : "FAIL")} {name}");
        if (!passed) failures++;
    }
}
