using System.Text;

var gateway = new PausedTelemetryGateway();
var publisher = new TelemetryPublisher(gateway);
var first = publisher.PublishAsync("BALLOON-A:18C");
var second = publisher.PublishAsync("BALLOON-B:21C");
gateway.Release();
await Task.WhenAll(first, second);

string[] source = ["BALLOON-A:18C", "BALLOON-B:21C"];
string[] received = gateway.Received.Order(StringComparer.Ordinal).ToArray();
Console.WriteLine("SOURCE: " + string.Join(" | ", source));
Console.WriteLine("RECEIVED: " + string.Join(" | ", received));
bool correct = source.Order(StringComparer.Ordinal).SequenceEqual(received);
string mode = args.FirstOrDefault() ?? "verify";
if (mode == "reproduce")
{
    Console.WriteLine(correct ? "NOT_REPRODUCED" : "REPRODUCED");
    Environment.ExitCode = correct ? 1 : 0;
}
else if (mode == "verify")
{
    Console.WriteLine(correct ? "PASS" : "FAIL: telemetry content changed");
    Environment.ExitCode = correct ? 0 : 1;
}
else
{
    Console.Error.WriteLine("Use reproduce or verify.");
    Environment.ExitCode = 2;
}

sealed class TelemetryPublisher(PausedTelemetryGateway gateway)
{
    private readonly byte[] _frameBuffer = new byte[64];

    public Task PublishAsync(string telemetry)
    {
        int length = Encoding.UTF8.GetBytes(telemetry.AsSpan(), _frameBuffer);
        return gateway.SendAsync(_frameBuffer.AsMemory(0, length));
    }
}

sealed class PausedTelemetryGateway
{
    private readonly TaskCompletionSource _release = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly List<string> _received = [];
    private readonly object _lock = new();
    public IReadOnlyList<string> Received { get { lock (_lock) return _received.ToArray(); } }
    public void Release() => _release.SetResult();
    public async Task SendAsync(ReadOnlyMemory<byte> frame)
    {
        await _release.Task;
        string value = Encoding.UTF8.GetString(frame.Span);
        lock (_lock) _received.Add(value);
    }
}
