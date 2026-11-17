using System.Text.Json;

var mode = args.Length == 1 ? args[0] : "verify";
if (mode is not ("verify" or "reproduce"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [verify|reproduce]");
    return 2;
}

var directory = Path.Combine(Path.GetTempPath(), "signal-config-lab-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(directory);
var path = Path.Combine(directory, "signal.json");
const string oldJson = "{\"version\":1,\"route\":\"A\"}";
const string newJson = "{\"version\":2,\"route\":\"B\"}";
try
{
    File.WriteAllText(path, oldJson);
    try { SnapshotWriter.Save(path, newJson, interrupt: true); }
    catch (IOException) { Console.WriteLine("Injected interruption observed."); }

    var oldSurvived = File.Exists(path) && File.ReadAllText(path) == oldJson;
    var jsonReadable = IsJson(path);
    Console.WriteLine($"After interrupted update: oldSurvived={oldSurvived}, jsonReadable={jsonReadable}, bytes={(File.Exists(path) ? new FileInfo(path).Length : -1)}");
    if (mode == "reproduce")
    {
        var symptom = !oldSurvived || !jsonReadable;
        Console.WriteLine(symptom ? "REPRODUCED" : "NOT REPRODUCED (possibly fixed)");
        return symptom ? 0 : 1;
    }

    if (!oldSurvived || !jsonReadable) { Console.Error.WriteLine("FAIL: failed update damaged last good snapshot"); return 1; }
    if (Directory.GetFiles(directory).Length != 1) { Console.Error.WriteLine("FAIL: temporary artifacts after interruption"); return 1; }
    SnapshotWriter.Save(path, newJson, interrupt: false);
    if (File.ReadAllText(path) != newJson || !IsJson(path)) { Console.Error.WriteLine("FAIL: successful update not committed"); return 1; }
    if (Directory.GetFiles(directory).Length != 1) { Console.Error.WriteLine("FAIL: temporary artifacts after success"); return 1; }
    Console.WriteLine("PASS: failure atomicity, successful update, and cleanup");
    return 0;
}
finally { Directory.Delete(directory, recursive: true); }

static bool IsJson(string path)
{
    try { using var doc = JsonDocument.Parse(File.ReadAllText(path)); return doc.RootElement.ValueKind == JsonValueKind.Object; }
    catch (Exception ex) when (ex is IOException or JsonException) { return false; }
}

static class SnapshotWriter
{
    public static void Save(string path, string json, bool interrupt)
    {
        if (interrupt)
        {
            File.WriteAllText(path, json[..(json.Length / 2)]);
            throw new IOException("Simulated write interruption");
        }
        File.WriteAllText(path, json);
    }
}
