using TrialImport;

internal static class Program
{
    private const string Fixture =
        "case_id,site,participant_note,status\r\n" +
        "TR-101,North,\"Reported nausea, mild\",review\r\n" +
        "TR-102,South,\"Follow-up says \"\"no symptoms\"\"\",approved\r\n" +
        "TR-103,East,\"Day 1\r\nDay 2\",review\r\n" +
        "TR-104,West,,approved\r\n";

    private static readonly string[][] Expected =
    [
        ["case_id", "site", "participant_note", "status"],
        ["TR-101", "North", "Reported nausea, mild", "review"],
        ["TR-102", "South", "Follow-up says \"no symptoms\"", "approved"],
        ["TR-103", "East", "Day 1\r\nDay 2", "review"],
        ["TR-104", "West", "", "approved"]
    ];

    private static int Main(string[] args)
    {
        return (args.FirstOrDefault() ?? "demo").ToLowerInvariant() switch
        {
            "demo" => Demo(),
            "reproduce" => Reproduce(),
            "verify" => Verify(),
            _ => Usage()
        };
    }

    private static int Usage()
    {
        Console.Error.WriteLine("Usage: dotnet run -- [demo|reproduce|verify]");
        return 2;
    }

    private static int Demo()
    {
        var records = CsvParser.ParseRecords(Fixture);
        Console.WriteLine($"Records={records.Count}; expected={Expected.Length}");
        foreach (var row in records.Take(7))
        {
            Console.WriteLine($"columns={row.Length} | " +
                string.Join(" | ", row.Select(x => x.Replace("\r", "\\r").Replace("\n", "\\n"))));
        }
        return 0;
    }

    private static int Reproduce()
    {
        var actual = CsvParser.ParseRecords(Fixture);
        var problems = new List<string>();
        Compare(actual, Expected, problems, "partner-fixture");
        Console.WriteLine($"Observed records={actual.Count}; expected={Expected.Length}; discrepancies={problems.Count}");
        foreach (var problem in problems.Take(8)) Console.WriteLine("EVIDENCE " + problem);
        if (problems.Count == 0)
        {
            Console.Error.WriteLine("NOT REPRODUCED: starter does not show the intended symptom.");
            return 1;
        }
        Console.WriteLine("REPRODUCED: imported record/field contract is violated.");
        return 0;
    }

    private static int Verify()
    {
        var problems = new List<string>();
        Compare(CsvParser.ParseRecords(Fixture), Expected, problems, "partner-fixture");
        Compare(CsvParser.ParseRecords("id,note\n5,\"hello, world\"\n"),
            [["id", "note"], ["5", "hello, world"]], problems, "quoted-delimiter");
        Compare(CsvParser.ParseRecords("id,note\r\n6,\"a\"\"b\"\r\n"),
            [["id", "note"], ["6", "a\"b"]], problems, "escaped-quote");
        Compare(CsvParser.ParseRecords("id,note\r\n7,\"line1\r\nline2\"\r\n"),
            [["id", "note"], ["7", "line1\r\nline2"]], problems, "embedded-newline");
        Compare(CsvParser.ParseRecords("id,note\n8,"),
            [["id", "note"], ["8", ""]], problems, "empty-final-field");
        Compare(CsvParser.ParseRecords("id,note\n9,plain"),
            [["id", "note"], ["9", "plain"]], problems, "no-trailing-newline");
        try
        {
            _ = CsvParser.ParseRecords("id,note\n10,\"unclosed");
            problems.Add("malformed-quote: parser silently accepted an unfinished field");
        }
        catch (FormatException) { /* Invalid CSV must be rejected. */ }

        foreach (var problem in problems.Take(25)) Console.Error.WriteLine("FAIL " + problem);
        if (problems.Count > 0)
        {
            Console.Error.WriteLine($"VERIFY FAILED: {problems.Count} discrepancies.");
            return 1;
        }
        Console.WriteLine("VERIFY PASSED: records, columns, exact field values and invalid-input behavior.");
        return 0;
    }

    private static void Compare(
        IReadOnlyList<string[]> actual, string[][] expected, List<string> problems, string scenario)
    {
        if (actual.Count != expected.Length)
            problems.Add($"{scenario}: record count {actual.Count}, expected {expected.Length}");

        for (var i = 0; i < Math.Min(actual.Count, expected.Length); i++)
        {
            if (actual[i].Length != expected[i].Length)
                problems.Add($"{scenario}: row {i} has {actual[i].Length} columns, expected {expected[i].Length}");

            for (var j = 0; j < Math.Min(actual[i].Length, expected[i].Length); j++)
            {
                if (!string.Equals(actual[i][j], expected[i][j], StringComparison.Ordinal))
                    problems.Add($"{scenario}: row {i} field {j} value differs");
            }
        }
    }
}
