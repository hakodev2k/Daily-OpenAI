using System.Threading.Channels;

var channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
{
    SingleReader = false,
    SingleWriter = true
});
await channel.Writer.WriteAsync("LAB-2026-119");
channel.Writer.TryComplete();

// Replay gate: hai worker đều quan sát trạng thái trước khi cùng tiếp tục.
var arrived = 0;
var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
var processed = 0;
var errors = 0;

async Task Worker(int workerId)
{
    if (await channel.Reader.WaitToReadAsync())
    {
        if (Interlocked.Increment(ref arrived) == 2) gate.TrySetResult();
        await gate.Task;
        if (!channel.Reader.TryRead(out var result))
        {
            Interlocked.Increment(ref errors);
            Console.WriteLine($"ERROR worker={workerId} queue-empty");
            return;
        }
        Interlocked.Increment(ref processed);
        Console.WriteLine($"PROCESSED worker={workerId} id={result}");
    }
    Console.WriteLine($"DONE worker={workerId}");
}

await Task.WhenAll(Worker(1), Worker(2));
Console.WriteLine($"RESULT processed={processed} errors={errors}");
if (args.Contains("--verify")) Environment.ExitCode = processed == 1 && errors == 0 ? 0 : 1;
