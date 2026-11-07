using System.Text.Json;
return await Runner.RunAsync(args);

internal static class Runner
{
    const string Old = "{\"revision\":41,\"gate\":\"closed\",\"release\":\"hold\"}";
    const string New = "{\"revision\":42,\"gate\":\"open\",\"release\":\"approved\"}";

    public static async Task<int> RunAsync(string[] args)
    {
        string mode = args.Length == 0 ? "observe" : args[0];
        if (mode is not ("observe" or "reproduce" or "verify")) return 2;
        string folder = Path.Combine(Path.GetTempPath(), "gate-lab-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        try
        {
            string file = Path.Combine(folder, "schedule.json");
            await File.WriteAllTextAsync(file, Old);
            var reached = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Task Checkpoint() { reached.TrySetResult(true); return release.Task; }
            Task publish = GateSchedulePublisher.PublishAsync(file, New, Checkpoint);
            Task first = await Task.WhenAny(reached.Task, Task.Delay(TimeSpan.FromSeconds(8)));
            if (first != reached.Task)
            {
                release.TrySetResult(true);
                await publish;
                throw new InvalidOperationException("Checkpoint was not reached.");
            }
            string during;
            try { during = await ReadAsync(file); }
            finally { release.TrySetResult(true); }
            await publish;
            string after = await ReadAsync(file);
            Console.WriteLine("BEFORE: " + Old);
            Console.WriteLine("DURING: " + during);
            Console.WriteLine("AFTER : " + after);
            Check(IsJson(Old) && IsJson(New) && after == New, "Fixture or final snapshot invalid.");
            if (mode == "observe") return 0;
            if (mode == "reproduce")
            {
                Check(during != Old && during != New && !IsJson(during), "Original failure not reproduced.");
                Console.WriteLine("REPRODUCED: partial JSON visible.");
                return 0;
            }
            Check(during == Old && IsJson(during), "Reader did not observe complete previous snapshot.");
            await File.WriteAllTextAsync(file, Old);
            bool rejected = false;
            try
            {
                await GateSchedulePublisher.PublishAsync(file, New,
                    () => Task.FromException(new InvalidOperationException("review rejected")));
            }
            catch (InvalidOperationException e) when (e.Message == "review rejected") { rejected = true; }
            Check(rejected, "Checkpoint rejection not propagated.");
            Check(await ReadAsync(file) == Old, "Rejected publish damaged previous snapshot.");
            Check(Directory.GetFiles(folder, "*.tmp").Length == 0, "Staging file leaked.");
            Console.WriteLine("VERIFIED: consistent snapshots and failure recovery.");
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine("FAILED: " + e.Message); return 1; }
        finally { Directory.Delete(folder, true); }
    }

    static async Task<string> ReadAsync(string file)
    {
        using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
    static bool IsJson(string value)
    {
        try { using var doc = JsonDocument.Parse(value); return doc.RootElement.ValueKind == JsonValueKind.Object; }
        catch (JsonException) { return false; }
    }
    static void Check(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException(message);
    }
}
