namespace CaptionLab;

public static class CaptionLocale
{
    private static readonly string DefaultLocale = "en";

    // Contract: supported caption locales are en, vi, fr.
    public static string? Select(string? acceptLanguage)
    {
        if (string.IsNullOrWhiteSpace(acceptLanguage))
            return DefaultLocale;

        string first = acceptLanguage.Split(',')[0].Split(';')[0].Trim();
        if (first.StartsWith("vi", StringComparison.OrdinalIgnoreCase))
            return "vi";
        if (first.StartsWith("fr", StringComparison.OrdinalIgnoreCase) && first.Length == 2)
            return "fr";
        return DefaultLocale;
    }
}
