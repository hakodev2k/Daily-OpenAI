using System.Text.RegularExpressions;

namespace MeterCodeLab;

// Reference only. Apply the concept to starter/MeterCodeValidator.cs yourself.
internal static class ReferenceMeterCodeValidator
{
    private static readonly Regex CodePattern = new(
        @"\A[A-Z0-9]+\z",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking,
        TimeSpan.FromMilliseconds(100));

    public static bool IsValid(string code)
    {
        ArgumentNullException.ThrowIfNull(code);
        return CodePattern.IsMatch(code);
    }
}
