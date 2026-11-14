using System;
using System.Collections.Generic;
using System.Linq;

public record VaccineBatch(string BatchId, int ExpiryDays, int Doses);

public static class Dispatch
{
    public static IReadOnlyList<VaccineBatch> Plan(IEnumerable<VaccineBatch> incoming)
    {
        var ordered = new SortedSet<VaccineBatch>(Comparer<VaccineBatch>.Create(
            (a, b) => { var c = a.ExpiryDays.CompareTo(b.ExpiryDays); return c != 0 ? c : StringComparer.Ordinal.Compare(a.BatchId, b.BatchId); }));
        foreach (var batch in incoming) ordered.Add(batch);
        return ordered.ToArray();
    }
}

public static class Program
{
    public static int Main(string[] args)
    {
        var batches = new[] {
            new VaccineBatch("V-201", 2, 100),
            new VaccineBatch("V-202", 2, 80),
            new VaccineBatch("V-203", 5, 60),
            new VaccineBatch("V-204", 1, 40)
        };
        var actual = Dispatch.Plan(batches);
        Console.WriteLine("INPUT=" + string.Join(",", batches.Select(x => x.BatchId)));
        Console.WriteLine("OUTPUT=" + string.Join(",", actual.Select(x => x.BatchId)));
        Console.WriteLine($"COUNT={actual.Count}; DOSES={actual.Sum(x => x.Doses)}");
        if (args.Contains("--verify"))
        {
            bool ok = actual.Count == batches.Length &&
                actual.Select(x => x.BatchId).OrderBy(x => x).SequenceEqual(batches.Select(x => x.BatchId).OrderBy(x => x)) &&
                actual.Select(x => x.ExpiryDays).SequenceEqual(actual.Select(x => x.ExpiryDays).OrderBy(x => x)) &&
                actual.Sum(x => x.Doses) == batches.Sum(x => x.Doses);
            var sameId = Dispatch.Plan(new[] { batches[0], batches[0] });
            ok &= sameId.Count == 1;
            Console.WriteLine(ok ? "VERIFY_PASS" : "VERIFY_FAIL");
            return ok ? 0 : 2;
        }
        if (args.Contains("--reproduce"))
        {
            bool broken = actual.Count != batches.Length || actual.Sum(x => x.Doses) != batches.Sum(x => x.Doses);
            Console.WriteLine(broken ? "REPRODUCED" : "NOT_REPRODUCED");
            return broken ? 0 : 3;
        }
        return 0;
    }
}
