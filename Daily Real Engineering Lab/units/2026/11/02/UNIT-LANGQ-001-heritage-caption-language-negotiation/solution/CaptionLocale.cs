using System.Net.Http.Headers;

namespace CaptionLab;

public static class CaptionLocale
{
    private static readonly string[] Supported = { "en", "vi", "fr" };
    private sealed record Preference(string Language, double Quality, int Position);

    public static string? Select(string? acceptLanguage)
    {
        if (string.IsNullOrWhiteSpace(acceptLanguage))
            return "en";

        var preferences = new List<Preference>();
        string[] segments = acceptLanguage.Split(',');
        for (int i = 0; i < segments.Length; i++)
        {
            if (!StringWithQualityHeaderValue.TryParse(segments[i].Trim(), out var parsed) || parsed is null)
                continue;

            string raw = parsed.Value;
            string language = raw == "*" ? "*" : raw.Split('-')[0].ToLowerInvariant();
            if (language != "*" && !IsLanguageToken(language))
                continue;

            double q = parsed.Quality ?? 1.0;
            if (double.IsNaN(q) || q < 0 || q > 1)
                continue;

            preferences.Add(new Preference(language, q, i));
        }

        var candidates = new List<(string Locale, double Quality, int Position, int Rank)>();
        for (int rank = 0; rank < Supported.Length; rank++)
        {
            string locale = Supported[rank];
            var explicitMatches = preferences.Where(p => p.Language == locale).ToList();
            var matches = explicitMatches.Count > 0
                ? explicitMatches
                : preferences.Where(p => p.Language == "*").ToList();

            var best = matches.OrderByDescending(p => p.Quality).ThenBy(p => p.Position).FirstOrDefault();
            if (best is not null && best.Quality > 0)
                candidates.Add((locale, best.Quality, best.Position, rank));
        }

        return candidates.OrderByDescending(x => x.Quality)
            .ThenBy(x => x.Position)
            .ThenBy(x => x.Rank)
            .Select(x => x.Locale)
            .FirstOrDefault();
    }

    private static bool IsLanguageToken(string value) =>
        value.Length is >= 2 and <= 8 && value.All(char.IsAsciiLetter);
}
