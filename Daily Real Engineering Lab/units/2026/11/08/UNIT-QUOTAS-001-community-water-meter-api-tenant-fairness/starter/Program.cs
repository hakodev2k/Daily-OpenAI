using System;
using System.Collections.Generic;
using System.Linq;

record Request(int Second, string Tenant, string MeterId);
record Decision(Request Request, bool Accepted, string Reason);

static class Program
{
    static readonly Request[] Trace =
    [
        new(0, "bulk", "B01"), new(0, "bulk", "B02"), new(0, "bulk", "B03"),
        new(0, "bulk", "B04"), new(0, "bulk", "B05"), new(0, "bulk", "B06"),
        new(0, "clinic", "C01"), new(0, "school", "S01"),
        new(1, "bulk", "B07"), new(1, "bulk", "B08"),
        new(1, "clinic", "C02"), new(1, "school", "S02")
    ];
    // Fixed two-second capacity window: total upstream service can process 8 requests.
    // Investigate whether the current admission policy meets service obligations.
    static IEnumerable<Decision> Admit(IEnumerable<Request> requests)
    {
        int accepted = 0;
        foreach (var request in requests)
        {
            if (accepted >= 8)
            {
                yield return new(request, false, "global-capacity");
                continue;
            }
            accepted++;
            yield return new(request, true, "ok");
        }
    }
    static int Main(string[] args)
    {
        var decisions = Admit(Trace).ToArray();
        foreach (var d in decisions)
            Console.WriteLine($"{d.Request.Second},{d.Request.Tenant},{d.Request.MeterId},{(d.Accepted ? 200 : 429)},{d.Reason}");
        Console.WriteLine("SUMMARY " + string.Join(" ", decisions.GroupBy(x => x.Request.Tenant)
            .OrderBy(x => x.Key).Select(g => $"{g.Key}={g.Count(x => x.Accepted)}/{g.Count()}")));
        if (args.Contains("--verify"))
        {
            var accepted = decisions.Where(x => x.Accepted).ToArray();
            bool capacity = accepted.Length <= 8;
            bool isolation = new[] { "clinic", "school" }.All(t =>
                accepted.Count(x => x.Request.Tenant == t) == 2);
            bool bulkHasService = accepted.Any(x => x.Request.Tenant == "bulk");
            Console.WriteLine($"CHECK capacity={capacity} isolation={isolation} bulkHasService={bulkHasService}");
            return capacity && isolation && bulkHasService ? 0 : 1;
        }
        return 0;
    }
}
