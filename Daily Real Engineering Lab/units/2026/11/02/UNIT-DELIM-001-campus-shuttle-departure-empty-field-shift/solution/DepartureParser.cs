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

        // The vendor contract has five positional fields; an empty note is meaningful.
        string[] parts = line.Split('|', StringSplitOptions.TrimEntries);

        if (parts.Length != 5)
            throw new FormatException("Expected exactly five pipe-delimited fields.");

        if (string.IsNullOrWhiteSpace(parts[0]) ||
            string.IsNullOrWhiteSpace(parts[1]) ||
            string.IsNullOrWhiteSpace(parts[3]) ||
            string.IsNullOrWhiteSpace(parts[4]))
        {
            throw new FormatException("A required departure field is empty.");
        }

        return new Departure(parts[0], parts[1], parts[2], parts[3], parts[4]);
    }
}
