var left = new Counter(1, 100);
var right = new Counter(2, 100);
Task first = Task.Run(() => Transfer(left, right, 10));
Task second = Task.Run(() => Transfer(right, left, 20));
await Task.WhenAll(first, second);
if (left.Balance < 0 || right.Balance < 0 || left.Balance + right.Balance != 200)
    throw new InvalidOperationException("Invariant violated");
Console.WriteLine($"SETTLED total={left.Balance + right.Balance} left={left.Balance} right={right.Balance}");

static void Transfer(Counter source, Counter destination, int amount)
{
    if (ReferenceEquals(source, destination)) throw new ArgumentException("Distinct counters required");
    var first = source.Id < destination.Id ? source : destination;
    var second = source.Id < destination.Id ? destination : source;
    lock (first.Sync)
    lock (second.Sync)
    {
        if (source.Balance < amount) throw new InvalidOperationException("Insufficient tickets");
        source.Balance -= amount;
        destination.Balance += amount;
    }
}
sealed class Counter(int id, int balance)
{
    public int Id { get; } = id;
    public int Balance { get; set; } = balance;
    public object Sync { get; } = new();
}

