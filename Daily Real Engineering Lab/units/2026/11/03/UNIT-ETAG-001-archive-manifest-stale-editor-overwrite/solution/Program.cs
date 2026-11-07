using System;
using System.Collections.Generic;

var store = new ManifestStore();
var alice = store.Read();
var bob = store.Read();
Console.WriteLine($"READ Alice revision={alice.Revision}; Bob revision={bob.Revision}");
var first = store.Save(new Manifest(alice.Revision, "Đã đối chiếu tác giả", alice.Location));
var second = store.Save(new Manifest(bob.Revision, bob.Note, "Kho B-12"));
var final = store.Read();
Console.WriteLine($"SAVE Alice={first}; Bob={second}");
Console.WriteLine($"FINAL revision={final.Revision} note={final.Note} location={final.Location}");
bool conflictRejected = first == SaveResult.Success && second == SaveResult.Conflict;
bool noLoss = final.Note == "Đã đối chiếu tác giả" && final.Location == "Kho A-01";
if (args.Contains("--verify"))
{
    // Behavioral contract: a stale full-document update must not erase the first writer.
    if (!conflictRejected || !noLoss) { Console.Error.WriteLine("VERIFY FAIL: stale editor overwrote a committed change"); Environment.Exit(1); }
    var current = store.Read();
    var retry = store.Save(current with { Location = "Kho B-12" });
    var afterRetry = store.Read();
    if (retry != SaveResult.Success || afterRetry.Note != "Đã đối chiếu tác giả" || afterRetry.Location != "Kho B-12" || afterRetry.Revision != 3)
    { Console.Error.WriteLine("VERIFY FAIL: reload/retry contract"); Environment.Exit(1); }
    Console.WriteLine("VERIFY PASS");
}
else if (args.Contains("--reproduce"))
{
    if (first == SaveResult.Success && second == SaveResult.Success && final.Note == "Chưa đối chiếu")
    { Console.WriteLine("REPRODUCED: silent lost update"); return; }
    Console.Error.WriteLine("REPRODUCE FAIL: expected baseline symptom not observed"); Environment.Exit(1);
}

public record Manifest(int Revision, string Note, string Location);
public enum SaveResult { Success, Conflict }

public sealed class ManifestStore
{
    private readonly object _gate = new();
    private Manifest _current = new(1, "Chưa đối chiếu", "Kho A-01");

    public Manifest Read() { lock (_gate) return _current with { }; }

    public SaveResult Save(Manifest submitted)
    {
        lock (_gate)
        {
            if (submitted.Revision != _current.Revision) return SaveResult.Conflict;
            _current = submitted with { Revision = _current.Revision + 1 };
            return SaveResult.Success;
        }
    }
}
