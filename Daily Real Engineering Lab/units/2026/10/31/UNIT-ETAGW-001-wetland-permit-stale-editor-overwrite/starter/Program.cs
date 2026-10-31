using System;
internal record Draft(string Body, string ETag);
internal record Result(int Status, string ETag);
internal sealed class DraftStore
{
    private readonly object _gate = new();
    private string _body = "Kế hoạch khảo sát bờ kè";
    private long _revision = 1;
    private string Tag() => $"\"rev-{_revision}\"";
    public Draft Get() { lock (_gate) return new Draft(_body, Tag()); }
    // Mô phỏng HTTP PUT boundary bằng response status và ETag.
    public Result Put(string? ifMatch, string body)
    {
        lock (_gate)
        {
            _body = body;
            _revision++;
            return new Result(200, Tag());
        }
    }
}
internal static class Program
{
    private const string A = "A: bảo vệ hành lang sinh thái 20m";
    private const string B = "B: mở đường khảo sát sát bờ kè";
    private static int Main(string[] args) => args.Length == 1 ? args[0] switch
    {
        "reproduce" => Reproduce(), "verify" => Verify(), _ => 64
    } : 64;
    private static int Reproduce()
    {
        var store = new DraftStore();
        var a = store.Get(); var b = store.Get();
        var first = store.Put(a.ETag, A); var second = store.Put(b.ETag, B);
        var final = store.Get();
        bool symptom = first.Status == 200 && second.Status == 200 && final.Body == B;
        Console.WriteLine($"GET A={a.ETag}, GET B={b.ETag}; PUT A={first.Status}, PUT B={second.Status}; final={final.Body}");
        Console.WriteLine(symptom ? "SYMPTOM_REPRODUCED" : "SYMPTOM_NOT_REPRODUCED");
        return symptom ? 0 : 2;
    }
    private static int Verify()
    {
        var store = new DraftStore();
        var a = store.Get(); var b = store.Get();
        var first = store.Put(a.ETag, A); var stale = store.Put(b.ETag, B);
        var afterConflict = store.Get();
        var missingStore = new DraftStore();
        var missing = missingStore.Put(null, B);
        var weakStore = new DraftStore();
        var weak = weakStore.Put("W/\"rev-1\"", B);
        var fresh = store.Put(afterConflict.ETag, B);
        var afterFresh = store.Get();
        bool pass = first.Status == 200 && stale.Status == 412
            && afterConflict.Body == A && afterConflict.ETag == first.ETag
            && missing.Status == 428 && missingStore.Get().Body == "Kế hoạch khảo sát bờ kè"
            && weak.Status == 412 && weakStore.Get().Body == "Kế hoạch khảo sát bờ kè"
            && fresh.Status == 200 && afterFresh.Body == B
            && afterFresh.ETag != afterConflict.ETag;
        Console.WriteLine($"first={first.Status}, stale={stale.Status}, missing={missing.Status}, weak={weak.Status}, fresh={fresh.Status}");
        Console.WriteLine($"afterConflict={afterConflict.Body}, afterFresh={afterFresh.Body}");
        Console.WriteLine(pass ? "CONTRACT_PASS" : "CONTRACT_FAIL");
        return pass ? 0 : 1;
    }
}
