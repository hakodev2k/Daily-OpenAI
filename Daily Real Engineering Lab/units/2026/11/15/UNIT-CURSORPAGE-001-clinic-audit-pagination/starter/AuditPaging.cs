using System;
using System.Collections.Generic;
using System.Linq;

public record AuditEvent(long Sequence, string Message);
public static class AuditPaging
{
    // API hiện nhận page number và page size từ dashboard.
    public static IReadOnlyList<AuditEvent> ReadPage(
        IEnumerable<AuditEvent> events, int page, int pageSize)
    {
        if (page < 1 || pageSize < 1) throw new ArgumentOutOfRangeException();
        return events.OrderByDescending(e => e.Sequence)
            .Skip((page - 1) * pageSize).Take(pageSize).ToArray();
    }
}
