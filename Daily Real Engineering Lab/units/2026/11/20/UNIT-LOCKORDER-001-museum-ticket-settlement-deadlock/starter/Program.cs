using System.Threading;

var left = new Counter(1, 100);
var right = new Counter(2, 100);
using var rendezvous = new Barrier(2);

Task first = Task.Run(() => Transfer(left, right, 10, rendezvous));
Task second = Task.Run(() => Transfer(right, left, 20, rendezvous));
await Task.WhenAll(first, second);

if (left.Balance < 0 || right.Balance < 0 || left.Balance + right.Balance != 200)
    throw new InvalidOperationException("Invariant violated");
Console.WriteLine($"SETTLED total={left.Balance + right.Balance} left={left.Balance} right={right.Balance}");

static void Transfer(Counter source, Counter destination, int amount, Barrier gate)
{
    Console.WriteLine($"BEGIN {source.Id}->{destination.Id}");
    lock (source.Sync)
    {
        if (!gate.SignalAndWait(TimeSpan.FromSeconds(3)))
            throw new TimeoutException("Workers did not reach the same phase");
        lock (destination.Sync)
        {
            if (source.Balance < amount) throw new InvalidOperationException("Insufficient tickets");
            source.Balance -= amount;
            destination.Balance += amount;
        }
    }
}
sealed class Counter(int id, int balance)
{
    public int Id { get; } = id;
    public int Balance { get; set; } = balance;
    public object Sync { get; } = new();
}

