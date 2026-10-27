using System.Text;
using System.Text.Json;

namespace MarineSurveyLab;

// Reference Solution — copy into starter/CursorPager.cs only after your own attempt.
public static class CursorPager
{
    private sealed record CursorKey(long UtcTicks, string ObservationId);

    public static Page FetchPage(IReadOnlyList<Observation> source, string? cursor, int pageSize)
    {
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

        CursorKey? after = null;
        if (!string.IsNullOrWhiteSpace(cursor))
        {
            try
            {
                var json = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
                after = JsonSerializer.Deserialize<CursorKey>(json)
                    ?? throw new FormatException("Cursor must contain a position.");
                if (string.IsNullOrWhiteSpace(after.ObservationId))
                    throw new FormatException("Cursor is missing ObservationId.");
            }
            catch (Exception ex) when (ex is FormatException or JsonException)
            {
                throw new ArgumentException("Invalid pagination cursor.", nameof(cursor), ex);
            }
        }

        var rows = source
            .Where(x => after is null
                || x.ObservedAtUtc.UtcTicks > after.UtcTicks
                || (x.ObservedAtUtc.UtcTicks == after.UtcTicks
                    && string.CompareOrdinal(x.ObservationId, after.ObservationId) > 0))
            .OrderBy(x => x.ObservedAtUtc.UtcTicks)
            .ThenBy(x => x.ObservationId, StringComparer.Ordinal)
            .Take(pageSize)
            .ToArray();

        string? next = null;
        if (rows.Length != 0)
        {
            var last = rows[^1];
            var position = new CursorKey(last.ObservedAtUtc.UtcTicks, last.ObservationId);
            next = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(position)));
        }
        return new Page(rows, next);
    }
}
