using System.Threading.Channels;

var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(2)
{
    SingleReader = true,
    SingleWriter = true,
    FullMode = BoundedChannelFullMode.DropOldest
});

var accepted = 0;
foreach (var sensorEventId in new[] { 101, 102, 103 })
{
    await channel.Writer.WriteAsync(sensorEventId);
    accepted++;
}
channel.Writer.TryComplete();

var persisted = new List<int>();
await foreach (var sensorEventId in channel.Reader.ReadAllAsync())
{
    persisted.Add(sensorEventId);
}

Console.WriteLine($"Accepted={accepted} Persisted={persisted.Count}");
Console.WriteLine($"Ids={string.Join(",", persisted)}");
