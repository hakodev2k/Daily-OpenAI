using System.Text.RegularExpressions;

namespace MeterCodeLab;

internal static class MeterCodeValidator
{
    // The meter registry accepts non-empty uppercase ASCII letters and digits.
    private static readonly Regex CodePattern = new(
        @"\A([A-Z0-9]+)+\z",
        RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(100));

    public static bool IsValid(string code)
    {
        ArgumentNullException.ThrowIfNull(code);
        return CodePattern.IsMatch(code);
    }
}
