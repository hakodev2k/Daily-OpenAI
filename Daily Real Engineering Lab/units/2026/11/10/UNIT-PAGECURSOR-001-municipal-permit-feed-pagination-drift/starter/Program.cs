using System;
using System.Collections.Generic;
using System.Linq;

record Permit(int Id, DateTimeOffset CreatedAt);

static class PermitFeed
{
    public static IEnumerable<Permit> Page(IReadOnlyList<Permit> permits, int pageNumber, int pageSize)
        => permits.OrderByDescending(p => p.CreatedAt)
                  .ThenByDescending(p => p.Id)
                  .Skip(pageNumber * pageSize).Take(pageSize).ToArray();

    // Learner-editable path. Return all initial records exactly once despite later inserts.
    public static IEnumerable<int> Pages(List<Permit> permits, Action<List<Permit>> betweenPages)
    {
        var ids = new List<int>();
        var pageNumber = 0;
        var first = Page(permits, pageNumber++, 2).ToArray();
        ids.AddRange(first.Select(x => x.Id));
        betweenPages(permits);
        while (true)
        {
            var page = Page(permits, pageNumber++, 2).ToArray();
            if (page.Length == 0) break;
            ids.AddRange(page.Select(x => x.Id));
        }
        return ids;
    }
}

static class Program
{
    private static readonly DateTimeOffset T = new(2026, 11, 10, 0, 0, 0, TimeSpan.Zero);
    static List<Permit> Seed() => new() { new(1,T), new(2,T.AddMinutes(1)), new(3,T.AddMinutes(2)), new(4,T.AddMinutes(3)), new(5,T.AddMinutes(4)) };
    static int Main(string[] args)
    {
        var data = Seed();
        var result = PermitFeed.Pages(data, items => items.Add(new Permit(6,T.AddMinutes(5)))).ToArray();
        Console.WriteLine("Initial IDs: 5,4,3,2,1");
        Console.WriteLine("Observed IDs: " + string.Join(",",result));
        if (args.Contains("--verify"))
        {
            bool pass = result.SequenceEqual(new[]{5,4,3,2,1});
            Console.WriteLine(pass ? "PASS" : "FAIL: snapshot pagination contract");
            return pass ? 0 : 1;
        }
        return 0;
    }
}
