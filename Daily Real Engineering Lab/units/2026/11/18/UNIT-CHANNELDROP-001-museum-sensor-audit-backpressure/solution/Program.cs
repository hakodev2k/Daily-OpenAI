using System.Threading.Channels;

var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(2)
{
    SingleReader = true,
    SingleWriter = true,
    FullMode = BoundedChannelFullMode.Wait
});
var persisted = new List<int>();
var consumer = Task.Run(async () =>
{
    await foreach (var id in channel.Reader.ReadAllAsync())
        persisted.Add(id);
});
var accepted = 0;
foreach (var id in new[] { 101, 102, 103 })
{
    await channel.Writer.WriteAsync(id);
    accepted++;
}
channel.Writer.TryComplete();
await consumer;
Console.WriteLine($"Accepted={accepted} Persisted={persisted.Count}");
Console.WriteLine($"Ids={string.Join(",", persisted)}");
