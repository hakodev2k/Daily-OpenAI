using System;
using System.Collections.Generic;
using System.Linq;

public static class VerificationRunner
{
    public static int Main()
    {
        var alerts = new List<Alert> {
            new(101,600,"A"), new(102,500,"B"), new(103,500,"C"),
            new(104,400,"D"), new(105,300,"E"), new(106,200,"F")
        };
        var original = alerts.Select(x=>x.Id).Order().ToArray();
        var session = AlertPager.Start(alerts);
        var all = new List<int>();
        for (int page=0;page<5;page++) {
            var batch = AlertPager.ReadPage(alerts,session,page,3);
            if(batch.Count==0) break;
            all.AddRange(batch.Select(x=>x.Id));
            if(page==0) alerts.Add(new Alert(107,650,"new"));
        }
        bool unique = all.Count == all.Distinct().Count();
        bool complete = original.SequenceEqual(all.Order());
        bool excludedNew = !all.Contains(107);
        bool ordered = all.SequenceEqual(all.OrderByDescending(id=>alerts.Single(x=>x.Id==id).ObservedAt).ThenByDescending(id=>id));
        Console.WriteLine($"unique={unique}; complete={complete}; excludesNew={excludedNew}; deterministicOrder={ordered}");
        if (!(unique && complete && excludedNew && ordered)) { Console.Error.WriteLine("FAIL: learner paging contract not satisfied"); return 1; }
        Console.WriteLine("PASS: learner paging contract");
        return 0;
    }
}
