using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        bool verify = args.Contains("--verify");
        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(2)
        {
            SingleReader = true,
            SingleWriter = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });
        var consumed = new List<int>();
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var consumer = Task.Run(async () =>
        {
            await release.Task;
            await foreach (int item in channel.Reader.ReadAllAsync())
                consumed.Add(item);
        });

        // A downstream telemetry sink pauses while a burst of four alerts arrives.
        int accepted = 0;
        var producer = Task.Run(async () =>
        {
            foreach (int alert in new[] { 101, 102, 103, 104 })
            {
                await channel.Writer.WriteAsync(alert);
                Interlocked.Increment(ref accepted);
            }
            channel.Writer.TryComplete();
        });
        if (verify)
        {
            // Deterministic pressure gate: after 100ms the producer should still
            // be waiting for the sink, rather than claiming all four accepted.
            Task first = await Task.WhenAny(producer, Task.Delay(1000));
            if (first == producer)
            {
                Console.WriteLine("FAIL: producer finished before downstream released capacity");
                release.TrySetResult();
                await consumer;
                return 1;
            }
        }
        else
        {
            await producer;
        }
        release.TrySetResult();
        await Task.WhenAll(producer, consumer);
        Console.WriteLine($"accepted={accepted} delivered={consumed.Count} ids={string.Join(",", consumed)}");
        bool ok = accepted == 4 && consumed.SequenceEqual(new[] { 101, 102, 103, 104 });
        Console.WriteLine(ok ? "PASS: all alerts delivered in order" : "FAIL: accepted alerts missing or reordered");
        return ok ? 0 : 1;
    }
}
