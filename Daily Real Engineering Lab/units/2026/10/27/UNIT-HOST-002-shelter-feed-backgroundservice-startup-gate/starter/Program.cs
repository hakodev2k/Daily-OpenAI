using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShelterFeedLab;

if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- reproduce|verify");
    return 2;
}

var mode = args[0];
var probe = new WarmupProbe();

using var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddSingleton(probe);
        services.AddHostedService<ShelterFeedWarmupService>();
    })
    .Build();

// Đặt StartAsync trên task riêng để harness có thể quan sát trạng thái
// mà không bị giữ lại bởi chính lời gọi đang đo.
var startTask = Task.Run(() => host.StartAsync());

try
{
    await probe.Entered.Task.WaitAsync(TimeSpan.FromSeconds(5));
}
catch (TimeoutException)
{
    probe.Release();
    Console.Error.WriteLine("FAIL: worker did not enter warmup gate.");
    return 1;
}

Console.WriteLine("EVENT warmup-entered");
var startupFinishedBeforeRelease =
    await Task.WhenAny(startTask, Task.Delay(TimeSpan.FromSeconds(3))) == startTask;

Console.WriteLine($"EVENT host-startup-before-release={startupFinishedBeforeRelease}");

probe.Release();
Console.WriteLine("EVENT gate-released");

try
{
    await startTask.WaitAsync(TimeSpan.FromSeconds(5));
    await probe.Completed.Task.WaitAsync(TimeSpan.FromSeconds(5));
    using var stopTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    await host.StopAsync(stopTimeout.Token);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"FAIL: startup/warmup/shutdown did not finish: {ex}");
    return 1;
}

Console.WriteLine("EVENT warmup-completed");
Console.WriteLine("EVENT host-stopped");

bool expected = mode == "reproduce"
    ? !startupFinishedBeforeRelease
    : startupFinishedBeforeRelease;

Console.WriteLine(expected
    ? $"PASS: {mode} contract satisfied."
    : $"FAIL: {mode} contract violated.");
return expected ? 0 : 1;
