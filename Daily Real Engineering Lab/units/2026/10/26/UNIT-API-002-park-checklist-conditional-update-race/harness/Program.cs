public sealed record ChecklistSnapshot(string Note, int Revision);
public sealed record PatchResult(int StatusCode, string ETag);

public interface IConcurrencyProbe
{
    Task PauseAsync();
}

// Deterministic test-only rendezvous; not an application-level locking mechanism.
public sealed class TwoArrivalProbe : IConcurrencyProbe
{
    private readonly TaskCompletionSource<bool> _release =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _arrived;

    public Task PauseAsync()
    {
        var count = Interlocked.Increment(ref _arrived);
        if (count == 2) _release.TrySetResult(true);
        return count <= 2
            ? _release.Task.WaitAsync(TimeSpan.FromSeconds(5))
            : Task.CompletedTask;
    }
}

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length != 1 || args[0] is not ("reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run --project starter -- reproduce|verify");
            return 2;
        }

        try
        {
            var store = new RevisionedChecklistStore(new TwoArrivalProbe());
            var first = store.Read();
            Check(first.Revision == 7, "initial revision must be 7");

            const string noteA = "Replace damaged lamp";
            const string noteB = "Close inspection: no fault";
            var left = store.TryPatchAsync("\"7\"", noteA);
            var right = store.TryPatchAsync("\"7\"", noteB);
            var replies = await Task.WhenAll(left, right);
            var current = store.Read();

            Console.WriteLine($"request-A={replies[0].StatusCode}, request-B={replies[1].StatusCode}");
            Console.WriteLine($"revision={current.Revision}, note={current.Note}");
            Check(current.Revision == 8, "exactly one revision increment is expected");
            Check(current.Note is noteA or noteB, "final note must be from a submitted edit");
            Check((await store.TryPatchAsync(null, "no-precondition")).StatusCode == 428,
                "missing If-Match must be rejected");
            Check((await store.TryPatchAsync("W/\"8\"", "weak-etag")).StatusCode == 400,
                "weak or malformed ETag must not be accepted by this simplified contract");
            Check((await store.TryPatchAsync("\"7\"", "stale-write")).StatusCode == 412,
                "stale ETag must be rejected");
            Check(store.Read().Note == current.Note, "rejected writes must not change data");

            if (args[0] == "reproduce")
            {
                Check(replies.All(x => x.StatusCode == 200),
                    "starter no longer reproduces the double-acceptance symptom");
                Console.WriteLine("REPRODUCED: two conflicting conditional edits both succeeded.");
            }
            else
            {
                Check(replies.Count(x => x.StatusCode == 200) == 1,
                    "exactly one edit should be accepted");
                Check(replies.Count(x => x.StatusCode == 412) == 1,
                    "exactly one competing edit should receive 412");
                var winner = replies[0].StatusCode == 200 ? noteA : noteB;
                Check(current.Note == winner, "stored note must match the accepted edit");
                Check(replies.Single(x => x.StatusCode == 200).ETag == "\"8\"",
                    "successful response must return the new ETag");
                Console.WriteLine("VERIFIED: one conditional edit wins; the other is rejected.");
            }

            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine("CHECK FAILED: " + e.Message);
            return 1;
        }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
