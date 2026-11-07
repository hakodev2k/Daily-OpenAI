using System.Text.RegularExpressions;

namespace RegexLab;

public static class SkuValidator
{
    private static readonly TimeSpan MatchBudget = TimeSpan.FromMilliseconds(100);

    public static bool IsValid(string? sku)
    {
        if (string.IsNullOrEmpty(sku) || sku.Length > 2048)
        {
            return false;
        }

        return Regex.IsMatch(
            sku,
            @"^([A-Z0-9]+)+$",
            RegexOptions.CultureInvariant,
            MatchBudget);
    }
}
