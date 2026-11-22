using System.Collections.Concurrent;
using System.Threading;

var ledger = new IssuanceLedger();
var service = new PassService(ledger);
var gate = new ManualResetEventSlim(false);
var a = Task.Run(() => { gate.Wait(); return service.GetOrIssue("MEMBER-42"); });
var b = Task.Run(() => { gate.Wait(); return service.GetOrIssue("MEMBER-42"); });
gate.Set();
var results = await Task.WhenAll(a, b);
Console.WriteLine($"RESULT ids={string.Join(",", results.Select(x => x.Serial))} issued={ledger.IssueCount} cache={service.CacheCount}");
if (args.Contains("--assert")) {
    if (results[0].Serial != results[1].Serial || ledger.IssueCount != 1)
        throw new InvalidOperationException("Invariant failed: one member should receive one issued pass.");
    Console.WriteLine("PASS");
}
public sealed record Pass(string Serial);
public sealed class IssuanceLedger {
    private int _issued;
    public int IssueCount => Volatile.Read(ref _issued);
    public Pass Issue(string member) {
        var number = Interlocked.Increment(ref _issued);
        Thread.Sleep(120);
        return new Pass($"{member}-PASS-{number}");
    }
}
public sealed class PassService {
    private readonly IssuanceLedger _ledger;
    private readonly ConcurrentDictionary<string, Pass> _passes = new();
    public PassService(IssuanceLedger ledger) => _ledger = ledger;
    public int CacheCount => _passes.Count;
    public Pass GetOrIssue(string member) => _passes.GetOrAdd(member, key => _ledger.Issue(key));
}
