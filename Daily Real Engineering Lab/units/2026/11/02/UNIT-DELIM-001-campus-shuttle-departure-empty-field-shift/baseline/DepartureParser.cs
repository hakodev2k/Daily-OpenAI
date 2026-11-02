namespace PlatformFeed;

public sealed record Departure(
    string ServiceId,
    string Platform,
    string Note,
    string Destination,
    string Status);

public static class DepartureParser
{
    public static Departure Parse(string line)
    {
        ArgumentNullException.ThrowIfNull(line);
        string[] parts = line.Split('|',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length is < 4 or > 5)
            throw new FormatException("Unsupported departure record shape.");

        return new Departure(
            parts[0],
            parts[1],
            parts[2],
            parts[3],
            parts.Length == 5 ? parts[4] : "UNKNOWN");
    }
}
