using System.Text;

// Bộ pool nhỏ mô phỏng khả năng tái sử dụng buffer; không phụ thuộc may rủi của ArrayPool.Shared.
var pool = new ReusableBufferPool();
var transmitter = new DeferredTransmitter();
var accepted = new List<string> { "ALERT", "CLEAR" };
foreach (var message in accepted)
{
    byte[] buffer = pool.Rent();
    int count = Encoding.ASCII.GetBytes(message, buffer);
    transmitter.Enqueue(buffer, count);
    Console.WriteLine($"queued={message}");
    pool.Return(buffer);
    Console.WriteLine("reused=buffer returned to pool");
}
var transmitted = transmitter.Drain();
for (int i = 0; i < accepted.Count; i++)
    Console.WriteLine($"accepted={accepted[i]}; transmitted={transmitted[i]}");
bool matches = accepted.SequenceEqual(transmitted);
string mode = args.FirstOrDefault() ?? "run";
if (mode == "reproduce")
{
    if (!matches) { Console.WriteLine("REPRODUCED"); return 0; }
    Console.Error.WriteLine("NOT_REPRODUCED"); return 1;
}
if (mode == "verify")
{
    if (matches) { Console.WriteLine("PASS: both payloads preserved"); return 0; }
    Console.Error.WriteLine("FAIL: payload integrity"); return 1;
}
return matches ? 0 : 2;

sealed class ReusableBufferPool
{
    private readonly Stack<byte[]> _free = new();
    public byte[] Rent() => _free.Count > 0 ? _free.Pop() : new byte[16];
    public void Return(byte[] buffer) => _free.Push(buffer);
}

sealed class DeferredTransmitter
{
    private readonly List<Func<string>> _pending = new();
    public void Enqueue(byte[] buffer, int count)
    {
        _pending.Add(() => Encoding.ASCII.GetString(buffer, 0, count));
    }
    public List<string> Drain() => _pending.Select(job => job()).ToList();
}
