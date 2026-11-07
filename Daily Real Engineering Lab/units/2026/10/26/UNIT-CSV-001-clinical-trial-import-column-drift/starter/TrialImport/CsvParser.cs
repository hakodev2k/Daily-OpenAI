namespace TrialImport;

public static class CsvParser
{
    // Legacy import adapter. Preserve the public contract while investigating
    // why a subset of partner files produces inconsistent field counts.
    public static IReadOnlyList<string[]> ParseRecords(string csv)
    {
        return csv.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimEnd('\r').Split(','))
            .ToArray();
    }
}
