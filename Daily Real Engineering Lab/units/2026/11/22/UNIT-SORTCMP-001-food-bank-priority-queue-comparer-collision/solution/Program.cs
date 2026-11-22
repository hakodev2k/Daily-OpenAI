using System;
using System.Collections.Generic;
using System.Linq;

record Dispatch(string RequestId, int Priority, int Boxes);

sealed class DispatchComparer : IComparer<Dispatch>
{
    public int Compare(Dispatch? x, Dispatch? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;
        int priorityOrder = y.Priority.CompareTo(x.Priority);
        return priorityOrder != 0 ? priorityOrder : StringComparer.Ordinal.Compare(x.RequestId, y.RequestId);
    }
}

static class Program
{
    static int Main(string[] args)
    {
        var requests = new[] {
            new Dispatch("A-101", 90, 4),
            new Dispatch("B-205", 90, 7),
            new Dispatch("C-300", 70, 2),
            new Dispatch("D-410", 90, 3)
        };
        var queue = new SortedSet<Dispatch>(new DispatchComparer());
        foreach (var request in requests)
        {
            bool accepted = queue.Add(request);
            Console.WriteLine($"ENQUEUE {request.RequestId} accepted={accepted}");
        }
        Console.WriteLine($"EXPECTED={requests.Length} ACTUAL={queue.Count}");
        foreach (var request in queue) Console.WriteLine($"DISPATCH {request.RequestId}");
        return args.Contains("--verify") ? (queue.Count == requests.Length && queue.Select(x=>x.RequestId).Distinct().Count()==requests.Length ? 0 : 1) : 0;
    }
}
