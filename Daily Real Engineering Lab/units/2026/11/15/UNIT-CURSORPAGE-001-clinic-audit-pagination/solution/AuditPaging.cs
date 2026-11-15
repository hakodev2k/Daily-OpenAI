using System;
using System.Collections.Generic;
using System.Linq;

public record AuditEvent(long Sequence, string Message);
public static class AuditPaging
{
    // Sequence phải unique và tăng đơn điệu; cursor thuộc cùng một snapshot.
    public static IReadOnlyList<AuditEvent> ReadPage(
        IEnumerable<AuditEvent> events, long? beforeExclusive,
        long snapshotMaxInclusive, int pageSize)
    {
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));
        return events.Where(e => e.Sequence <= snapshotMaxInclusive
                    && (!beforeExclusive.HasValue || e.Sequence < beforeExclusive.Value))
            .OrderByDescending(e => e.Sequence).Take(pageSize).ToArray();
    }
}
