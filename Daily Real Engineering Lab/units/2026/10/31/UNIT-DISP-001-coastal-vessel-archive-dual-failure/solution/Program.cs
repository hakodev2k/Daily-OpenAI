using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

// Các fixture mô phỏng hai pha của một lần xuất hồ sơ.
// Chỉ sửa ArchiveExporter.ExportAsync khi thực hành.
var mode = args.FirstOrDefault()?.ToLowerInvariant() ?? "demo";
if (mode == "reproduce")
{
    var sink = new ArchiveSink(failUpload: true, failFinalize: true);
    var observed = await CaptureAsync(sink);
    Console.WriteLine($"Telemetry: upload attempt={sink.UploadCalls}, finalization={sink.DisposeCalls}");
    Console.WriteLine($"Observed error: {observed?.GetType().Name ?? "none"}");
    var reproduced = observed is ArchiveFinalizeException
        && sink.UploadCalls == 1 && sink.DisposeCalls == 1;
    Console.WriteLine(reproduced
        ? "REPRODUCED: incident report contains only finalization failure."
        : "NOT REPRODUCED: original symptom absent.");
    return reproduced ? 0 : 1;
}

var scenarios = new[]
{
    new Scenario("both", true, true),
    new Scenario("upload-only", true, false),
    new Scenario("finalize-only", false, true),
    new Scenario("success", false, false)
};
var failures = 0;
foreach (var scenario in scenarios)
{
    var sink = new ArchiveSink(scenario.FailUpload, scenario.FailFinalize);
    var observed = await CaptureAsync(sink);
    bool expected = scenario.Name switch
    {
        "both" => observed is AggregateException aggregate
            && aggregate.InnerExceptions.Count == 2
            && aggregate.InnerExceptions[0] is ArchiveUploadException
            && aggregate.InnerExceptions[1] is ArchiveFinalizeException,
        "upload-only" => observed is ArchiveUploadException,
        "finalize-only" => observed is ArchiveFinalizeException,
        "success" => observed is null,
        _ => false
    };
    bool once = sink.UploadCalls == 1 && sink.DisposeCalls == 1;
    bool ok = expected && once;
    Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {scenario.Name}: error={observed?.GetType().Name ?? "none"}, upload={sink.UploadCalls}, finalize={sink.DisposeCalls}");
    if (!ok) failures++;
}
if (mode == "verify")
{
    Console.WriteLine($"Verification: {scenarios.Length - failures}/{scenarios.Length} passed.");
    return failures == 0 ? 0 : 1;
}
if (mode != "demo")
{
    Console.Error.WriteLine("Use demo, reproduce or verify.");
    return 2;
}
Console.WriteLine("Demo only; run verify after modifying the starter.");
return 0;

static async Task<Exception?> CaptureAsync(ArchiveSink sink)
{
    try
    {
        await ArchiveExporter.ExportAsync(sink);
        return null;
    }
    catch (Exception ex)
    {
        return ex;
    }
}

internal sealed record Scenario(string Name, bool FailUpload, bool FailFinalize);

internal sealed class ArchiveUploadException : Exception
{
    public ArchiveUploadException() : base("Object storage rejected archive upload (HTTP 503).") { }
}
internal sealed class ArchiveFinalizeException : Exception
{
    public ArchiveFinalizeException() : base("Archive stream finalization failed.") { }
}
internal sealed class ArchiveSink : IAsyncDisposable
{
    private readonly bool _failUpload;
    private readonly bool _failFinalize;
    public int UploadCalls { get; private set; }
    public int DisposeCalls { get; private set; }

    public ArchiveSink(bool failUpload, bool failFinalize)
    {
        _failUpload = failUpload;
        _failFinalize = failFinalize;
    }

    public Task UploadAsync()
    {
        UploadCalls++;
        if (_failUpload) throw new ArchiveUploadException();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        DisposeCalls++;
        if (_failFinalize) throw new ArchiveFinalizeException();
        return ValueTask.CompletedTask;
    }
}

internal static class ArchiveExporter
{
    public static async Task ExportAsync(ArchiveSink sink)
    {
        ExceptionDispatchInfo? uploadFailure = null;
        ExceptionDispatchInfo? cleanupFailure = null;

        try
        {
            await sink.UploadAsync();
        }
        catch (Exception ex)
        {
            uploadFailure = ExceptionDispatchInfo.Capture(ex);
        }

        try
        {
            await sink.DisposeAsync();
        }
        catch (Exception ex)
        {
            cleanupFailure = ExceptionDispatchInfo.Capture(ex);
        }

        if (uploadFailure is not null && cleanupFailure is not null)
            throw new AggregateException(
                "Archive upload and finalization both failed.",
                uploadFailure.SourceException, cleanupFailure.SourceException);

        if (uploadFailure is not null) uploadFailure.Throw();
        if (cleanupFailure is not null) cleanupFailure.Throw();
    }
}
