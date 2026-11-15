using System;
using System.Linq;
var source = Enumerable.Range(1, 8).Select(i => new AuditEvent(i, $"audit-{i}")).ToList();
var first = AuditPaging.ReadPage(source, 1, 3);
source.Add(new AuditEvent(9, "late-write"));
var second = AuditPaging.ReadPage(source, 2, 3);
Console.WriteLine("page1=" + string.Join(",", first.Select(x => x.Sequence)));
Console.WriteLine("page2=" + string.Join(",", second.Select(x => x.Sequence)));
Console.WriteLine("duplicates=" + string.Join(",", first.Select(x => x.Sequence).Intersect(second.Select(x => x.Sequence))));
