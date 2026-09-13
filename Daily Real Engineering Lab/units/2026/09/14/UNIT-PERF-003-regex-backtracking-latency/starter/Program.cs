using System.Diagnostics;
using System.Text.RegularExpressions;

const string Pattern = @"^(A+)+Z$";

var validator = new ExternalCodeValidator();
var cases = new[]
{
    (Name: "valid", Value: "AAAAAZ"),
    (Name: "early-reject", Value: "BAAAAA"),
    (Name: "near-match", Value: new string('A', 28) + "!")
};

var failed = false;
foreach (var item in cases)
{
    var sw = Stopwatch.StartNew();
    try
    {
        var matched = validator.IsValid(item.Value);
        sw.Stop();
        Console.WriteLine($"{item.Name}: matched={matched}; elapsedMs={sw.ElapsedMilliseconds}");

        if (args.Contains("--assert-fixed") && item.Name == "near-match" && sw.ElapsedMilliseconds > 200)
            failed = true;
    }
    catch (RegexMatchTimeoutException)
    {
        sw.Stop();
        Console.WriteLine($"{item.Name}: TIMEOUT; elapsedMs={sw.ElapsedMilliseconds}");
        if (args.Contains("--assert-fixed"))
            failed = true;
    }
}

if (args.Contains("--assert-fixed") && failed)
    Environment.Exit(1);

internal sealed class ExternalCodeValidator
{
    private static readonly Regex CodeRegex = new(
        Pattern,
        RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(100));

    public bool IsValid(string value)
    {
        // Investigation note: the business rule is simply one-or-more 'A' characters followed by 'Z'.
        // Inspect how work grows for different kinds of rejected input before changing the implementation.
        return CodeRegex.IsMatch(value);
    }
}
