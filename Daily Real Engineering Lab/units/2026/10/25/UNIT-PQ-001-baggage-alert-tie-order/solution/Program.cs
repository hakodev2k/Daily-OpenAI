using System;
using System.Collections.Generic;
using System.Linq;

return Cli.Run(args);

internal sealed record BaggageAlert(string Id, int Severity);

internal static class DispatchPlanner
{
    // Contract: smaller severity first; for equal severity, preserve arrival order.
    public static IReadOnlyList<BaggageAlert> Plan(IEnumerable<BaggageAlert> arrivals)
    {
        var queue = new PriorityQueue<BaggageAlert, (int Severity, long Sequence)>();
        long sequence = 0;
        foreach (var alert in arrivals)
        {
            queue.Enqueue(alert, (alert.Severity, sequence++));
        }

        var result = new List<BaggageAlert>();
        while (queue.TryDequeue(out var alert, out _))
        {
            result.Add(alert);
        }

        return result;
    }
}

internal static class Cli
{
    public static int Run(string[] args) => (args.FirstOrDefault() ?? "demo") switch
    {
        "demo" => Demo(),
        "reproduce" => Reproduce(),
        "verify" => Verify(),
        _ => Usage()
    };

    private static int Usage()
    {
        Console.Error.WriteLine("Usage: demo | reproduce | verify");
        return 64;
    }

    private static int Demo()
    {
        var arrivals = new[]
        {
            new BaggageAlert("A", 0),
            new BaggageAlert("B", 0),
            new BaggageAlert("C", 0),
            new BaggageAlert("D", 0)
        };
        Console.WriteLine("Arrival: " + Ids(arrivals));
        Console.WriteLine("Dispatch: " + Ids(DispatchPlanner.Plan(arrivals)));
        return 0;
    }

    private static int Reproduce()
    {
        var arrivals = new[]
        {
            new BaggageAlert("A", 0),
            new BaggageAlert("B", 0),
            new BaggageAlert("C", 0),
            new BaggageAlert("D", 0)
        };
        var actual = DispatchPlanner.Plan(arrivals).Select(a => a.Id).ToArray();
        var expected = new[] { "A", "B", "C", "D" };
        Console.WriteLine("Expected: " + string.Join(",", expected));
        Console.WriteLine("Actual:   " + string.Join(",", actual));
        if (!actual.SequenceEqual(expected))
        {
            Console.WriteLine("REPRODUCED: equal-severity arrival-order contract violated.");
            return 0;
        }

        Console.WriteLine("NOT REPRODUCED: the original symptom is absent.");
        return 2;
    }

    private static int Verify()
    {
        var failed = 0;
        Check("equal-severity FIFO", new[]
        {
            new BaggageAlert("A", 0),
            new BaggageAlert("B", 0),
            new BaggageAlert("C", 0),
            new BaggageAlert("D", 0)
        }, new[] { "A", "B", "C", "D" });

        Check("severity dominates arrival time", new[]
        {
            new BaggageAlert("N1", 1),
            new BaggageAlert("U1", 0),
            new BaggageAlert("N2", 1),
            new BaggageAlert("U2", 0)
        }, new[] { "U1", "U2", "N1", "N2" });

        Check("three severity bands", new[]
        {
            new BaggageAlert("M1", 1),
            new BaggageAlert("L1", 2),
            new BaggageAlert("U1", 0),
            new BaggageAlert("M2", 1),
            new BaggageAlert("U2", 0),
            new BaggageAlert("L2", 2),
            new BaggageAlert("M3", 1),
            new BaggageAlert("U3", 0)
        }, new[] { "U1", "U2", "U3", "M1", "M2", "M3", "L1", "L2" });

        Check("empty queue", Array.Empty<BaggageAlert>(), Array.Empty<string>());

        Console.WriteLine(failed == 0 ? "VERIFIED: all contract cases passed." : $"FAILED: {failed} contract case(s).");
        return failed == 0 ? 0 : 1;

        void Check(string name, BaggageAlert[] arrivals, string[] expected)
        {
            var actual = DispatchPlanner.Plan(arrivals).Select(a => a.Id).ToArray();
            var pass = actual.SequenceEqual(expected);
            Console.WriteLine($"{(pass ? "PASS" : "FAIL")} {name}: expected=[{string.Join(",", expected)}] actual=[{string.Join(",", actual)}]");
            if (!pass) failed++;
        }
    }

    private static string Ids(IEnumerable<BaggageAlert> alerts) => string.Join(",", alerts.Select(a => a.Id));
}
