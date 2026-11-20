using System.Threading.Channels;
var log = new List<string>();
var accepted = new[] { "north-104", "south-205" };
var processed = new List<string>();
var channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
async Task North() { log.Add("north accepted"); await channel.Writer.WriteAsync(accepted[0]); log.Add("north queued"); }
async Task South() { log.Add("south accepted"); await gate.Task; try { await channel.Writer.WriteAsync(accepted[1]); log.Add("south queued"); } catch (ChannelClosedException) { log.Add("south write rejected"); } }
async Task Consume() { await foreach(var item in channel.Reader.ReadAllAsync()) { processed.Add(item); log.Add("read " + item); } log.Add("reader drained"); }
var reader = Consume();
var north = North();
var south = South();
await north;
channel.Writer.TryComplete();
log.Add("writer complete requested");
gate.SetResult();
await south;
await reader;
Console.WriteLine(string.Join(Environment.NewLine, log));
Console.WriteLine($"accepted={accepted.Length}; processed={processed.Count}; missing={string.Join(",", accepted.Except(processed))}");
if(args.Contains("--check") && (processed.Count != accepted.Length || !accepted.Order().SequenceEqual(processed.Order()))) { Console.Error.WriteLine("INVARIANT FAILED"); return 1; }
return 0;
