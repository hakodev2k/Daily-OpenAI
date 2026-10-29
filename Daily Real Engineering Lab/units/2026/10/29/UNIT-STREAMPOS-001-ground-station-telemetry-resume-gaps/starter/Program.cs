using System.Text;
using StreamCheckpointLab;

string command = args.Length == 0 ? "run" : args[0].ToLowerInvariant();
if (command is not ("run" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [run|reproduce|verify]");
    return 2;
}

string temp = Path.Combine(Path.GetTempPath(), "stream-checkpoint-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(temp);
try
{
    string feed = Path.Combine(temp, "ground-station-feed.txt");
    string cursor = Path.Combine(temp, "checkpoint.txt");
    string[] expected =
    [
        "GS-101|antenna-lock",
        "GS-102|signal-acquired",
        "GS-103|packet-verified",
        "GS-104|pass-completed"
    ];
    File.WriteAllLines(feed, expected, new UTF8Encoding(false));

    // New importer instance on each batch simulates a worker restart.
    var first = new FeedImporter().Import(feed, cursor, 1);
    var second = new FeedImporter().Import(feed, cursor, 2);
    var third = new FeedImporter().Import(feed, cursor, 2);
    string[] all = first.Concat(second).Concat(third).ToArray();

    Console.WriteLine($"SourceLines={expected.Length}; SourceBytes={new FileInfo(feed).Length}");
    Console.WriteLine($"Batch1=[{string.Join(",", first)}]");
    Console.WriteLine($"Batch2=[{string.Join(",", second)}]");
    Console.WriteLine($"Batch3=[{string.Join(",", third)}]");
    Console.WriteLine($"AcceptedTotal={all.Length}; Cursor={File.ReadAllText(cursor)}");

    if (command == "reproduce")
    {
        bool symptom = first.SequenceEqual(expected.Take(1)) &&
                       second.Count == 0 && third.Count == 0;
        Console.WriteLine(symptom ? "REPRODUCED: unprocessed records missing" :
                                    "NOT REPRODUCED: original symptom absent");
        return symptom ? 0 : 1;
    }

    if (command == "verify")
    {
        if (!all.SequenceEqual(expected))
        {
            Console.Error.WriteLine("FAIL: initial batches must yield all four events exactly once and in order.");
            return 1;
        }

        var atEof = new FeedImporter().Import(feed, cursor, 2);
        if (atEof.Count != 0)
        {
            Console.Error.WriteLine("FAIL: no event should reappear at EOF.");
            return 1;
        }

        File.AppendAllText(feed, "GS-105|archive-confirmed" + Environment.NewLine,
            new UTF8Encoding(false));
        var appended = new FeedImporter().Import(feed, cursor, 1);
        if (!appended.SequenceEqual(["GS-105|archive-confirmed"]))
        {
            Console.Error.WriteLine("FAIL: appended event must be accepted exactly once.");
            return 1;
        }

        if (new FeedImporter().Import(feed, cursor, 1).Count != 0)
        {
            Console.Error.WriteLine("FAIL: appended event was replayed.");
            return 1;
        }

        Console.WriteLine("PASS: all events recovered, no duplicate, append-only continuation works.");
        return 0;
    }

    Console.WriteLine("RUN complete; use reproduce and verify for explicit assertions.");
    return 0;
}
finally
{
    Directory.Delete(temp, recursive: true);
}
