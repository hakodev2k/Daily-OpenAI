using System.Text.Json;

namespace PortManifestLab;

internal static class Program
{
    private const string Normal =
        """{"vesselId":"VN-604","cargoCode":"CHEM-77","requiresInspection":true}""";
    private const string Ambiguous =
        """{"vesselId":"VN-604","cargoCode":"CHEM-77","requiresInspection":true,"RequiresInspection":false}""";
    private const string ExactDuplicate =
        """{"vesselId":"VN-604","cargoCode":"CHEM-77","requiresInspection":true,"requiresInspection":false}""";
    private const string ValidFalse =
        """{"vesselId":"VN-604","cargoCode":"FOOD-11","requiresInspection":false}""";
    private const string Uppercase =
        """{"VESSELID":"VN-604","CARGOCODE":"CHEM-77","REQUIRESINSPECTION":true}""";
    private const string ExtraMetadata =
        """{"vesselId":"VN-604","cargoCode":"CHEM-77","requiresInspection":true,"partnerVersion":2}""";

    private static int Main(string[] args)
    {
        try
        {
            return args.Length == 1 ? args[0] switch
            {
                "--run" => Run(),
                "--reproduce" => Reproduce(),
                "--verify" => Verify(),
                _ => Usage()
            } : Usage();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }

    private static int Usage()
    {
        Console.Error.WriteLine("Use --run, --reproduce, or --verify.");
        return 2;
    }

    private static int Run()
    {
        Console.WriteLine("PARTNER_DECLARATION_1=" + Normal);
        Console.WriteLine("PARTNER_DECLARATION_2=" + Ambiguous);
        Console.WriteLine("DECISION_1=" + ManifestIntake.Parse(Normal).RequiresInspection);
        Console.WriteLine("DECISION_2=" + ManifestIntake.Parse(Ambiguous).RequiresInspection);
        return 0;
    }

    private static int Reproduce()
    {
        var baseline = ManifestIntake.Parse(Normal);
        var second = ManifestIntake.Parse(Ambiguous);
        var downgraded = baseline.RequiresInspection && !second.RequiresInspection;
        Console.WriteLine("BASELINE_REQUIRES_INSPECTION=" + baseline.RequiresInspection);
        Console.WriteLine("AMBIGUOUS_ACCEPTED=True");
        Console.WriteLine("INSPECTION_DOWNGRADED=" + downgraded);
        if (!downgraded) return 1;
        Console.WriteLine("REPRODUCED: conflicting partner decision was accepted.");
        return 0;
    }

    private static int Verify()
    {
        var passed = 0;

        void Check(bool condition, string label)
        {
            if (!condition) throw new InvalidOperationException(label);
            passed++;
            Console.WriteLine("PASS: " + label);
        }

        void MustReject(string payload, string label)
        {
            try
            {
                _ = ManifestIntake.Parse(payload);
            }
            catch (JsonException)
            {
                passed++;
                Console.WriteLine("PASS: " + label);
                return;
            }

            throw new InvalidOperationException(label + " must be rejected with JsonException.");
        }

        Check(ManifestIntake.Parse(Normal).RequiresInspection,
            "ordinary true declaration");
        Check(!ManifestIntake.Parse(ValidFalse).RequiresInspection,
            "legitimate false declaration");
        Check(ManifestIntake.Parse(Uppercase).RequiresInspection,
            "legacy casing remains supported");
        Check(ManifestIntake.Parse(ExtraMetadata).RequiresInspection,
            "non-conflicting extension field remains supported");
        MustReject(Ambiguous, "conflicting logical field names");
        MustReject(ExactDuplicate, "repeated exact field name");
        MustReject("[]", "non-object root");
        MustReject("{\\\"vesselId\\\":", "malformed JSON");

        Console.WriteLine("CHECKS_PASSED=" + passed);
        if (passed != 8) return 1;
        Console.WriteLine("VERIFIED: ingress contract and valid payload compatibility.");
        return 0;
    }
}
