using System.Threading.Channels;

const int expected = 8;
var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(3)
{
    SingleReader = true,
    SingleWriter = true,
    FullMode = BoundedChannelFullMode.DropOldest
});

for (var sequence = 1; sequence <= expected; sequence++)
{
    var accepted = channel.Writer.TryWrite(sequence);
    Console.WriteLine($"producer accepted={accepted} sequence={sequence}");
}
channel.Writer.TryComplete();

var observed = new List<int>();
await foreach (var sequence in channel.Reader.ReadAllAsync())
    observed.Add(sequence);

Console.WriteLine($"audit count={observed.Count} sequences={string.Join(",", observed)}");
var complete = observed.SequenceEqual(Enumerable.Range(1, expected));
Console.WriteLine($"audit complete={complete}");
if (args.Contains("--verify") && !complete)
    Environment.ExitCode = 1;
