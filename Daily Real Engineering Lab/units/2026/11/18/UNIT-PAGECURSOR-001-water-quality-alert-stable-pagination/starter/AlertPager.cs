using System;
using System.Collections.Generic;
using System.Linq;

public sealed record Alert(int Id, long ObservedAt, string Message);
public sealed record PageSession(long MaxObservedAt, int MaxId);

public static class AlertPager
{
    // A session identifies the initial high-water mark. Review the paging contract.
    public static PageSession Start(IReadOnlyCollection<Alert> source)
    {
        var top = source.OrderByDescending(x => x.ObservedAt).ThenByDescending(x => x.Id).First();
        return new PageSession(top.ObservedAt, top.Id);
    }

    public static IReadOnlyList<Alert> ReadPage(
        IReadOnlyCollection<Alert> source, PageSession session, int pageIndex, int pageSize)
    {
        if (pageIndex < 0 || pageSize <= 0) throw new ArgumentOutOfRangeException();
        return source.OrderByDescending(x => x.ObservedAt)
            .Skip(checked(pageIndex * pageSize)).Take(pageSize).ToArray();
    }
}
