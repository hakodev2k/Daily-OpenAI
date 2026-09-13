using System.Diagnostics;
using System.Text.RegularExpressions;

var validator = new ExternalCodeValidator();
var cases = new[]
{
    (Name: "valid", Value: "AAAAAZ"),
    (Name: "early-reject", Value: "BAAAAA"),
    (Name: "near-match", Value: new string('A', 28) + "!")
};

foreach (var item in cases)
{
    var sw = Stopwatch.StartNew();
    var matched = validator.IsValid(item.Value);
    sw.Stop();
    Console.WriteLine($"{item.Name}: matched={matched}; elapsedMs={sw.ElapsedMilliseconds}");
}

internal sealed class ExternalCodeValidator
{
    private static readonly Regex CodeRegex = new(
        @"^A+Z$",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking,
        TimeSpan.FromMilliseconds(100));

    public bool IsValid(string value) => CodeRegex.IsMatch(value);
}
