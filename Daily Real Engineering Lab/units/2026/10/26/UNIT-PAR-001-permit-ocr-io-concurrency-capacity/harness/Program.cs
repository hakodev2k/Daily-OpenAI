using System.Diagnostics;
using System.Text.Json;

if (args.Length != 2 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: <reproduce|verify> <starter-csproj>");
    return 2;
}
var mode = args[0];
var project = Path.GetFullPath(args[1]);
if (!File.Exists(project))
{
    Console.Error.WriteLine("Starter project not found: " + project);
    return 2;
}
try
{
    var eight = await Execute(8);
    Print("N=8", eight);
    if (mode == "reproduce")
    {
        var reproduced = Healthy(eight) && eight.MaxActive <= 2 && eight.EffectiveConcurrency <= 2;
        Console.WriteLine(reproduced ? "REPRODUCED: partner capacity underused." : "NOT REPRODUCED");
        return reproduced ? 0 : 1;
    }
    var three = await Execute(3);
    var sixteen = await Execute(16);
    Print("N=3", three);
    Print("N=16", sixteen);
    var pass = Healthy(eight) && Healthy(three) && Healthy(sixteen)
        && eight.MaxActive >= 6 && eight.MaxActive <= 8 && eight.EffectiveConcurrency == 8
        && three.MaxActive == 3 && three.EffectiveConcurrency == 3
        && sixteen.MaxActive <= 8 && sixteen.EffectiveConcurrency == 8;
    Console.WriteLine(pass ? "VERIFIED: bounded, configurable I/O concurrency." : "FAILED: inspect metrics.");
    return pass ? 0 : 1;
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
    return 2;
}

static bool Healthy(Metrics m) => m.ProcessedCount == 24 && m.DuplicateCount == 0 &&
    m.MissingCount == 0 && m.RejectedCount == 0;

static void Print(string label, Metrics m) =>
    Console.WriteLine($"{label}: effective={m.EffectiveConcurrency}, peak={m.MaxActive}, processed={m.ProcessedCount}, rejected={m.RejectedCount}, missing={m.MissingCount}, duplicates={m.DuplicateCount}, elapsedMs={m.ElapsedMs}");

async Task<Metrics> Execute(int concurrency)
{
    using var process = new Process();
    process.StartInfo = new ProcessStartInfo("dotnet")
    {
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (var part in new[] { "run", "--project", project, "--", "--max-concurrency", concurrency.ToString() })
        process.StartInfo.ArgumentList.Add(part);
    if (!process.Start()) throw new InvalidOperationException("Cannot start dotnet.");
    var outputTask = process.StandardOutput.ReadToEndAsync();
    var errorTask = process.StandardError.ReadToEndAsync();
    await process.WaitForExitAsync();
    var output = await outputTask;
    var error = await errorTask;
    if (process.ExitCode != 0)
        throw new InvalidOperationException($"Starter exit={process.ExitCode}\n{error}\n{output}");
    var line = output.Split('\n').Select(x => x.Trim()).LastOrDefault(x => x.StartsWith("{"));
    if (line is null) throw new InvalidOperationException("No metrics JSON: " + output);
    return JsonSerializer.Deserialize<Metrics>(line, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidOperationException("Invalid JSON.");
}
sealed record Metrics(int RequestedConcurrency, int EffectiveConcurrency, int MaxActive,
    int ProcessedCount, int DuplicateCount, int RejectedCount, int MissingCount, long ElapsedMs);
