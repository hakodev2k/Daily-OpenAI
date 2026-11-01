using System.Text;
using CoastalAlertLab;

if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project starter -- reproduce|verify");
    return 2;
}

const string oldJson = "{\"revision\":17,\"level\":\"WATCH\",\"coast\":\"north\"}";
const string newJson = "{\"revision\":18,\"level\":\"WARNING\",\"coast\":\"north\"}";
string folder = Path.Combine(Path.GetTempPath(), "coastal-alert-lab-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(folder);
string target = Path.Combine(folder, "current-alert.json");

try
{
    File.WriteAllText(target, oldJson, new UTF8Encoding(false));
    using var entered = new ManualResetEventSlim(false);
    using var release = new ManualResetEventSlim(false);
    var publisher = new SnapshotPublisher();

    Task writer = Task.Run(() => publisher.Publish(target, newJson, () =>
    {
        entered.Set();
        if (!release.Wait(TimeSpan.FromSeconds(10)))
            throw new TimeoutException("Publisher barrier timed out.");
    }));

    if (!entered.Wait(TimeSpan.FromSeconds(10)))
    {
        release.Set();
        await writer;
        throw new TimeoutException("Publisher did not reach the observation barrier.");
    }

    string during;
    try { during = ReadSnapshot(target); }
    finally { release.Set(); }
    await writer;

    string after = ReadSnapshot(target);
    bool visibilityOk = during == oldJson || during == newJson;
    bool commitOk = after == newJson;

    File.WriteAllText(target, oldJson, new UTF8Encoding(false));
    bool failureObserved = false;
    try
    {
        publisher.Publish(target, newJson,
            () => throw new InvalidOperationException("Simulated exporter failure"));
    }
    catch (InvalidOperationException ex) when (ex.Message == "Simulated exporter failure")
    {
        failureObserved = true;
    }

    bool rollbackOk = ReadSnapshot(target) == oldJson;
    bool clean = !Directory.EnumerateFiles(folder, "*.tmp").Any();
    Console.WriteLine($"DURING bytes={Encoding.UTF8.GetByteCount(during)} value={during}");
    Console.WriteLine($"AFTER revision18={commitOk}");
    Console.WriteLine($"INTERRUPTED oldRevisionPreserved={rollbackOk} injectedException={failureObserved}");
    Console.WriteLine($"TEMP clean={clean}");
    Console.WriteLine($"CONTRACT visibility={visibilityOk} commit={commitOk} rollback={rollbackOk} cleanup={clean}");

    if (args[0] == "reproduce")
    {
        bool reproduced = !visibilityOk && commitOk && failureObserved && !rollbackOk;
        Console.WriteLine(reproduced ? "REPRODUCED: two contracts fail." : "NOT REPRODUCED.");
        return reproduced ? 0 : 1;
    }

    bool verified = visibilityOk && commitOk && failureObserved && rollbackOk && clean;
    Console.WriteLine(verified ? "VERIFIED: contracts pass." : "FAILED: publication contract regression.");
    return verified ? 0 : 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"HARNESS ERROR: {ex.GetType().Name}: {ex.Message}");
    return 2;
}
finally
{
    try { Directory.Delete(folder, recursive: true); }
    catch (IOException) { Console.Error.WriteLine("WARN: temp cleanup failed."); }
    catch (UnauthorizedAccessException) { Console.Error.WriteLine("WARN: temp cleanup denied."); }
}

static string ReadSnapshot(string path)
{
    using var stream = new FileStream(path, FileMode.Open, FileAccess.Read,
        FileShare.ReadWrite | FileShare.Delete);
    using var reader = new StreamReader(stream, new UTF8Encoding(false),
        detectEncodingFromByteOrderMarks: false);
    return reader.ReadToEnd();
}
