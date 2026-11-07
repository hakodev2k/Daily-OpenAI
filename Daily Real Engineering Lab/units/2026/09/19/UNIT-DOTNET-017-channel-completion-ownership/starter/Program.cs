using System.Threading.Channels;

var channel = Channel.CreateUnbounded<string>();
var processed = 0;

async Task ProduceAsync(string prefix)
{
    for (var i = 1; i <= 3; i++)
    {
        await channel.Writer.WriteAsync($"{prefix}-{i}");
        Console.WriteLine($"produced={prefix}-{i}");
    }
}

async Task ConsumeAsync()
{
    await foreach (var item in channel.Reader.ReadAllAsync())
    {
        Interlocked.Increment(ref processed);
        Console.WriteLine($"processed={item}");
    }
}

var consumer = ConsumeAsync();
var producers = Task.WhenAll(ProduceAsync("api"), ProduceAsync("worker"));
await producers;
Console.WriteLine("producers=completed");

var finished = await Task.WhenAny(consumer, Task.Delay(500));
Console.WriteLine($"processed-count={processed}");
Console.WriteLine($"consumer-completed={consumer.IsCompleted}");

if (args.Contains("reproduce"))
{
    if (processed == 6 && finished != consumer)
    {
        Console.WriteLine("REPRODUCED");
        return 2;
    }
    Console.WriteLine("REPRODUCE_FAILED");
    return 1;
}

if (args.Contains("verify"))
{
    if (processed == 6 && finished == consumer && consumer.IsCompletedSuccessfully)
    {
        Console.WriteLine("VERIFY_PASS");
        return 0;
    }
    Console.WriteLine("VERIFY_FAIL");
    return 3;
}

return 0;
