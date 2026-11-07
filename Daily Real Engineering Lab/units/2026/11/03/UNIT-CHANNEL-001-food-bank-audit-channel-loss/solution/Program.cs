using System.Threading.Channels;

const int expected = 8;
var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(3)
{
    SingleReader = true,
    SingleWriter = true,
    FullMode = BoundedChannelFullMode.Wait
});
var observed = new List<int>();
var consumer = Task.Run(async () =>
{
    await foreach (var sequence in channel.Reader.ReadAllAsync())
        observed.Add(sequence);
});
for (var sequence = 1; sequence <= expected; sequence++)
{
    await channel.Writer.WriteAsync(sequence);
    Console.WriteLine($"producer accepted=True sequence={sequence}");
}
channel.Writer.TryComplete();
await consumer;
Console.WriteLine($"audit count={observed.Count} sequences={string.Join(",", observed)}");
var complete = observed.SequenceEqual(Enumerable.Range(1, expected));
Console.WriteLine($"audit complete={complete}");
if (args.Contains("--verify") && !complete)
    Environment.ExitCode = 1;
