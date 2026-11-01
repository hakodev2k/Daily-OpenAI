using System.Globalization;
using AccessCodeLab;

// Contract harness: do not edit. Change only AccessCodeRegistry.cs.
internal static class Program
{
    private sealed record ProbeResult(
        string Locale, bool AsciiLookup, bool LookalikeLookup, bool DuplicateAccepted);

    private static int Main(string[] args)
    {
        if (args.Length > 1 ||
            (args.Length == 1 && args[0] is not ("--run" or "--reproduce" or "--verify")))
        {
            Console.Error.WriteLine("Usage: --run | --reproduce | --verify");
            return 2;
        }

        var en = Probe("en-US");
        var tr = Probe("tr-TR");
        Print(en);
        Print(tr);

        if (args.Length == 0 || args[0] == "--run") return 0;

        if (args[0] == "--reproduce")
        {
            bool observed = en.AsciiLookup && !en.DuplicateAccepted &&
                            !tr.AsciiLookup && tr.LookalikeLookup &&
                            tr.DuplicateAccepted;
            Console.WriteLine(observed
                ? "REPRODUCED: locale-dependent admission and duplicate detection."
                : "NOT REPRODUCED: original starter symptom absent.");
            return observed ? 0 : 1;
        }

        bool verified = en.AsciiLookup && !en.LookalikeLookup && !en.DuplicateAccepted &&
                        tr.AsciiLookup && !tr.LookalikeLookup && !tr.DuplicateAccepted;
        Console.WriteLine(verified
            ? "VERIFIED: ASCII pass-code identity contract holds across both locales."
            : "FAILED: pass-code identity contract differs or accepts a lookalike.");
        return verified ? 0 : 1;
    }

    private static ProbeResult Probe(string locale)
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(locale);
            var registry = new AccessCodeRegistry();
            if (!registry.TryAdd("INV-42", "BALCONY-12"))
                throw new InvalidOperationException("Seed ticket could not be registered.");

            bool asciiLookup = registry.TryGetSeat("inv-42", out var seat) &&
                               seat == "BALCONY-12";
            // First character is U+0131 LATIN SMALL LETTER DOTLESS I, not ASCII 'i'.
            bool lookalikeLookup = registry.TryGetSeat("\u0131NV-42", out _);
            bool duplicateAccepted = registry.TryAdd("inv-42", "BALCONY-13");
            return new ProbeResult(locale, asciiLookup, lookalikeLookup, duplicateAccepted);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    private static void Print(ProbeResult result) =>
        Console.WriteLine($"{result.Locale}: asciiLookup={result.AsciiLookup} " +
                          $"lookalikeLookup={result.LookalikeLookup} " +
                          $"duplicateAccepted={result.DuplicateAccepted}");
}
