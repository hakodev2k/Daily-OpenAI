using System.Text;
using PermitIntake;

internal static class Program
{
    private static UploadCandidate Make(string name, string type, string text) =>
        new(name, type, Encoding.ASCII.GetBytes(text));

    private static int Main(string[] args)
    {
        if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
        {
            Console.Error.WriteLine("Usage: reproduce | verify");
            return 2;
        }

        var valid = Make("plan.pdf", "application/pdf", "%PDF-1.7\n1 0 obj\n<<>>\nendobj\n%%EOF");
        var disguised = Make("plan.pdf", "application/pdf", "MZ\u0000\u0002not-a-pdf");

        if (args[0] == "reproduce")
        {
            var admitted = UploadGate.Validate(disguised);
            var control = UploadGate.Validate(valid);
            Console.WriteLine($"admission={admitted.Code}; control={control.Code}; downstream=DOCUMENT_PARSE_FAILED");
            if (admitted.Accepted && control.Accepted)
            {
                Console.WriteLine("REPRODUCED: mismatched document entered downstream queue.");
                return 0;
            }
            Console.Error.WriteLine("Baseline symptom not reproduced.");
            return 1;
        }

        int failures = 0;
        void Check(string name, bool passed)
        {
            Console.WriteLine($"{(passed ? "PASS" : "FAIL")} {name}");
            if (!passed) failures++;
        }

        Check("valid PDF admission", UploadGate.Validate(valid).Accepted);
        Check("disguised document rejected", !UploadGate.Validate(disguised).Accepted);
        Check("short payload rejected", !UploadGate.Validate(Make("plan.pdf", "application/pdf", "%PDF")).Accepted);
        Check("near-miss header rejected", !UploadGate.Validate(Make("plan.pdf", "application/pdf", "%PDFXnot-pdf")).Accepted);
        Check("wrong extension rejected", !UploadGate.Validate(Make("plan.txt", "application/pdf", "%PDF-1.7")).Accepted);
        Check("wrong media type rejected", !UploadGate.Validate(Make("plan.pdf", "text/plain", "%PDF-1.7")).Accepted);
        Check("empty file rejected", !UploadGate.Validate(new("plan.pdf", "application/pdf", Array.Empty<byte>())).Accepted);
        Check("oversized file rejected", !UploadGate.Validate(new("plan.pdf", "application/pdf", new byte[1_048_577])).Accepted);
        Console.WriteLine(failures == 0 ? "VERIFIED" : $"FAILED: {failures} contracts");
        return failures == 0 ? 0 : 1;
    }
}
