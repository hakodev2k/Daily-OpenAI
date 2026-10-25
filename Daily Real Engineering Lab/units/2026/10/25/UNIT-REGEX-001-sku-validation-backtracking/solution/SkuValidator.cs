using System.Text.RegularExpressions;

namespace RegexLab;

public static class SkuValidator
{
    private static readonly Regex AllowedSku = new(
        @"\A[A-Z0-9]+\z",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking,
        TimeSpan.FromMilliseconds(100));

    public static bool IsValid(string? sku)
    {
        if (string.IsNullOrEmpty(sku) || sku.Length > 2048)
        {
            return false;
        }

        return AllowedSku.IsMatch(sku);
    }
}
