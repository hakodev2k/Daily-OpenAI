using System.Threading.Channels;

const int totalItems = 200;
const int consumerDelayMs = 8;
const int capacity = 8;

var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(capacity)
{
    SingleReader = true,
    SingleWriter = true,
    FullMode = BoundedChannelFullMode.Wait
});

var maxBacklog = 0;
var produced = 0;
var consumed = 0;

var consumer = Task.Run(async () =>
{
    await foreach (var item in channel.Reader.ReadAllAsync())
    {
        _ = item;
        await Task.Delay(consumerDelayMs);
        consumed++;
    }
});

var producerStarted = DateTime.UtcNow;
for (var i = 0; i < totalItems; i++)
{
    await channel.Writer.WriteAsync(i);
    produced++;

    if (channel.Reader.CanCount)
    {
        maxBacklog = Math.Max(maxBacklog, channel.Reader.Count);
    }
}
var producerElapsed = DateTime.UtcNow - producerStarted;

channel.Writer.Complete();
await consumer;

Console.WriteLine($"produced={produced}");
Console.WriteLine($"consumed={consumed}");
Console.WriteLine($"maxBacklog={maxBacklog}");
Console.WriteLine($"producerElapsedMs={(int)producerElapsed.TotalMilliseconds}");
