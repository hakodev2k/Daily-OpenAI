using ShelterLab;
var start = DateTimeOffset.Parse("2026-10-31T22:00:00-04:00");
var end = DateTimeOffset.Parse("2026-11-01T06:00:00-05:00");
var controlStart = DateTimeOffset.Parse("2026-10-24T22:00:00-04:00");
var controlEnd = DateTimeOffset.Parse("2026-10-25T06:00:00-04:00");
var a = StayDuration.BillableHours(start, end);
var b = StayDuration.BillableHours(controlStart, controlEnd);
bool rejected = false;
try { StayDuration.BillableHours(end, start); } catch (ArgumentException) { rejected = true; }
Console.WriteLine($"DST actual={a} expected=9");
Console.WriteLine($"CONTROL actual={b} expected=8");
Console.WriteLine($"NEGATIVE_REJECTED={rejected}");
return a == 9m && b == 8m && rejected ? 0 : 1;
