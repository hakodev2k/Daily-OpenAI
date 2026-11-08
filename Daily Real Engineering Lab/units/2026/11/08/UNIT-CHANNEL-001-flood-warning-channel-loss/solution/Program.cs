using System.Threading.Channels;

const int total = 12;
var queue = Channel.CreateBounded<int>(new BoundedChannelOptions(3)
{
    SingleReader = true,
    SingleWriter = true,
    FullMode = BoundedChannelFullMode.Wait
});
var delivered = new List<int>();
var consumer = Task.Run(async () =>
{
    await foreach (var alert in queue.Reader.ReadAllAsync())
    {
        delivered.Add(alert);
        await Task.Yield();
    }
});
for (var i = 1; i <= total; i++)
    await queue.Writer.WriteAsync(i);
queue.Writer.TryComplete();
await consumer;
Console.WriteLine($"Published={total}; Delivered={delivered.Count}; IDs={string.Join(",", delivered)}");
bool ok = delivered.Count == total && delivered.SequenceEqual(Enumerable.Range(1, total));
if (args.Contains("--verify"))
{
    if (!ok) { Console.Error.WriteLine("FAIL: lost or reordered alert."); return 1; }
    Console.WriteLine("PASS: all alerts delivered in order.");
    return 0;
}
if (ok) { Console.Error.WriteLine("No loss reproduced (expected after fix)."); return 1; }
return 0;
