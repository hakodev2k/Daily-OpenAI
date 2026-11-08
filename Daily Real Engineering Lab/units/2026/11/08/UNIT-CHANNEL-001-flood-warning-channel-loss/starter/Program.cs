using System.Threading.Channels;

const int total = 12;
var queue = Channel.CreateBounded<int>(new BoundedChannelOptions(3)
{
    SingleReader = true,
    SingleWriter = true,
    FullMode = BoundedChannelFullMode.DropOldest
});

var delivered = new List<int>();
// A downstream gateway can pause during maintenance.
var producer = Task.Run(async () =>
{
    for (var i = 1; i <= total; i++)
        await queue.Writer.WriteAsync(i);
    queue.Writer.TryComplete();
});
await producer;
await foreach (var alert in queue.Reader.ReadAllAsync())
{
    delivered.Add(alert);
}
Console.WriteLine($"Published={total}; Delivered={delivered.Count}; IDs={string.Join(",", delivered)}");
if (args.Contains("--verify"))
{
    bool ok = delivered.Count == total &&
              delivered.SequenceEqual(Enumerable.Range(1, total));
    if (!ok)
    {
        Console.Error.WriteLine("FAIL: at least one published alert was not delivered in order.");
        return 1;
    }
    Console.WriteLine("PASS: all alerts delivered in order.");
    return 0;
}
if (delivered.Count == total)
{
    Console.Error.WriteLine("Reproduction did not show alert loss.");
    return 1;
}
Console.WriteLine("REPRODUCED: downstream received fewer alerts than published.");
return 0;
