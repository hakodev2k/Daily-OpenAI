using System.Globalization;

public record Sample(string SampleId, string Site, string? PesticidePpm, string Status);

public static class Importer
{
    // Partner contract: exactly four comma-delimited fields; no quoted commas.
    public static Sample Parse(string line)
    {
        string[] parts = line.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length != 4) throw new FormatException("Sample record is incomplete");
        return new Sample(parts[0], parts[1], parts[2], parts[3]);
    }
}

public static class Program
{
    public static int Main(string[] args)
    {
        string[] lines = args.Contains("--verify", StringComparer.Ordinal)
            ? new[] { "S-101,North,0.04,PASS", "S-102,South,,PENDING", "S-103,West,0.01,PASS" }
            : new[] { "S-101,North,0.04,PASS", "S-102,South,,PENDING" };
        int bad = 0;
        foreach (string line in lines)
        {
            try
            {
                Sample row = Importer.Parse(line);
                bool correct = row.SampleId == line.Split(',')[0]
                    && row.Site == line.Split(',')[1]
                    && row.PesticidePpm == (line.Split(',')[2] == "" ? "" : line.Split(',')[2])
                    && row.Status == line.Split(',')[3];
                Console.WriteLine($"{row.SampleId}: pesticide={row.PesticidePpm ?? "<null>"}; status={row.Status}; valid={correct}");
                if (!correct) bad++;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Rejected: {ex.Message}");
                bad++;
            }
        }
        if (args.Contains("--verify", StringComparer.Ordinal))
        {
            bool malformedRejected;
            try { Importer.Parse("S-999,East,0.02,PASS,EXTRA"); malformedRejected = false; }
            catch (FormatException) { malformedRejected = true; }
            Console.WriteLine($"malformedRejected={malformedRejected}");
            return bad == 0 && malformedRejected ? 0 : 1;
        }
        Console.WriteLine($"invalidRows={bad}");
        return bad > 0 ? 2 : 0;
    }
}
