using System.Globalization;

namespace PermitRegistryLab;

internal static class Program
{
    private static readonly PermitRevision[] Feed =
    {
        new("INSP-204", "RECEIVED"),
        new("insp-204", "CLEARED"),
        new("INSP-205", "PENDING")
    };

    private static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "--diagnose" : args[0];
        return mode switch
        {
            "--diagnose" => Diagnose(),
            "--reproduce" => Reproduce(),
            "--verify" => Verify(),
            _ => InvalidMode(mode)
        };
    }

    private static int InvalidMode(string mode)
    {
        Console.Error.WriteLine("Unknown mode: " + mode);
        Console.Error.WriteLine("Use --diagnose, --reproduce or --verify.");
        return 2;
    }

    private static Observation Inspect(string cultureName)
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            IReadOnlyDictionary<string, PermitRevision> index =
                PermitRegistry.Reconcile(Feed);

            string status204 = index.TryGetValue("INSP-204", out var p204)
                ? p204.Status : "MISSING";
            string status205 = index.TryGetValue("INSP-205", out var p205)
                ? p205.Status : "MISSING";

            return new Observation(cultureName, index.Count, status204, status205);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    private static void Print(Observation item)
    {
        Console.WriteLine("locale=" + item.Locale
            + " count=" + item.Count
            + " INSP-204=" + item.Status204
            + " INSP-205=" + item.Status205);
    }

    private static int Diagnose()
    {
        Print(Inspect("en-US"));
        Print(Inspect("tr-TR"));
        Print(Inspect("vi-VN"));
        Console.WriteLine("Diagnostic only; run --reproduce or --verify for assertions.");
        return 0;
    }

    private static int Reproduce()
    {
        Observation en = Inspect("en-US");
        Observation tr = Inspect("tr-TR");
        Print(en);
        Print(tr);

        bool baseline = en.IsCorrect;
        bool symptom = tr.Count == 3
            && tr.Status204 == "RECEIVED"
            && tr.Status205 == "PENDING";

        if (baseline && symptom)
        {
            Console.WriteLine("REPRODUCED: identical feed, different registry result.");
            return 0;
        }

        Console.Error.WriteLine("Symptom not reproduced. Check starter and globalization settings.");
        return 1;
    }

    private static int Verify()
    {
        Observation[] observations =
        {
            Inspect("en-US"),
            Inspect("tr-TR"),
            Inspect("vi-VN")
        };

        foreach (Observation observation in observations)
            Print(observation);

        if (observations.All(x => x.IsCorrect))
        {
            Console.WriteLine("PASS: stable identity, latest revision and independent ID.");
            return 0;
        }

        Console.Error.WriteLine("FAIL: at least one locale violates the permit identity contract.");
        return 1;
    }

    private sealed record Observation(
        string Locale, int Count, string Status204, string Status205)
    {
        public bool IsCorrect => Count == 2
            && Status204 == "CLEARED"
            && Status205 == "PENDING";
    }
}
