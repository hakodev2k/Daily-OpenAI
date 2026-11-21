using System;
using System.Collections.Generic;

record Snapshot(int Remaining, int Version);
sealed class SeatStore
{
    private int remaining = 1;
    private int version = 1;
    public Snapshot Read() => new(remaining, version);
    public bool TryWrite(Snapshot observed, int next)
    {
        if (next < 0 || observed.Version != version || observed.Remaining != remaining) return false;
        // A persistence adapter simplified for local investigation.
        remaining = next;
        version++;
        return true;
    }
    public Snapshot Current => Read();
}
class Program
{
    static int Main(string[] args)
    {
        var store = new SeatStore();
        var first = store.Read();
        var second = store.Read();
        var acceptedA = first.Remaining > 0 && store.TryWrite(first, first.Remaining - 1);
        var acceptedB = second.Remaining > 0 && store.TryWrite(second, second.Remaining - 1);
        Console.WriteLine($"accepted={Convert.ToInt32(acceptedA)+Convert.ToInt32(acceptedB)} remaining={store.Current.Remaining} version={store.Current.Version}");
        if (args.Length > 0 && args[0] == "--verify")
        {
            if (Convert.ToInt32(acceptedA)+Convert.ToInt32(acceptedB) != 1 || store.Current.Remaining != 0) return 1;
            Console.WriteLine("PASS: capacity preserved");
        }
        return 0;
    }
}
