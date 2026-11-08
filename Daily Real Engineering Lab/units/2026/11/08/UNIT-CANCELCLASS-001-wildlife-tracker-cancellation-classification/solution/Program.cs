using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

var mode = args.Length == 0 ? "reproduce" : args[0];
var scenarios = new[] { Scenario.CallerCanceled, Scenario.DependencyTimeout, Scenario.Success };
var failures = 0;
foreach (var scenario in scenarios)
{
    var transport = new FakeTransport();
    using var caller = new CancellationTokenSource();
    if (scenario == Scenario.CallerCanceled) caller.Cancel();
    var result = await TrackerUploader.UploadAsync(transport, scenario, caller.Token);
    var expected = scenario switch
    {
        Scenario.CallerCanceled => UploadStatus.Canceled,
        Scenario.DependencyTimeout => UploadStatus.TimedOut,
        _ => UploadStatus.Sent
    };
    Console.WriteLine($"scenario={scenario} status={result} expected={expected} attempts={transport.Attempts}");
    if (result != expected || transport.Attempts != 1) failures++;
}
if (mode == "verify" && failures != 0) Environment.ExitCode = 1;
else if (mode == "reproduce" && failures == 0) Environment.ExitCode = 2;
else if (mode == "reproduce") Console.WriteLine("REPRODUCED: cancellation outcome contract violated");

enum Scenario { CallerCanceled, DependencyTimeout, Success }
enum UploadStatus { Sent, Canceled, TimedOut }
sealed class FakeTransport
{
    public int Attempts { get; private set; }
    public Task SendAsync(Scenario scenario, CancellationToken token)
    {
        Attempts++;
        if (scenario == Scenario.CallerCanceled) return Task.FromCanceled(token);
        if (scenario == Scenario.DependencyTimeout) return Task.FromException(new OperationCanceledException("uplink deadline exceeded"));
        return Task.CompletedTask;
    }
}
static class TrackerUploader
{
    public static async Task<UploadStatus> UploadAsync(FakeTransport transport, Scenario scenario, CancellationToken callerToken)
    {
        try
        {
            await transport.SendAsync(scenario, callerToken);
            return UploadStatus.Sent;
        }
        catch (OperationCanceledException) when (callerToken.IsCancellationRequested)
        {
            return UploadStatus.Canceled;
        }
        catch (OperationCanceledException)
        {
            return UploadStatus.TimedOut;
        }
    }
}
