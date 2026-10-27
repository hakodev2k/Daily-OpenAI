using System.Globalization;

namespace MarineSurveyLab;

// Learner-editable: giữ nguyên public method signature và Page contract.
public static class CursorPager
{
    public static Page FetchPage(IReadOnlyList<Observation> source, string? cursor, int pageSize)
    {
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

        DateTimeOffset? after = string.IsNullOrWhiteSpace(cursor)
            ? null
            : DateTimeOffset.Parse(cursor, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        var rows = source
            .Where(x => after is null || x.ObservedAtUtc > after.Value)
            .OrderBy(x => x.ObservedAtUtc)
            .ThenBy(x => x.ObservationId, StringComparer.Ordinal)
            .Take(pageSize)
            .ToArray();

        var next = rows.Length == 0
            ? null
            : rows[^1].ObservedAtUtc.ToString("O", CultureInfo.InvariantCulture);

        return new Page(rows, next);
    }
}
