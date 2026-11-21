using System;
using System.Collections.Generic;

// The input is a clinic-local wall-clock time, not an instant.
var zone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
var cases = new (string Name, DateTime Local, string Expected)[] {
    ("normal", new DateTime(2026, 10, 20, 9, 0, 0, DateTimeKind.Unspecified), "ACCEPT"),
    ("spring-gap", new DateTime(2026, 3, 8, 2, 30, 0, DateTimeKind.Unspecified), "INVALID"),
    ("fall-fold", new DateTime(2026, 11, 1, 1, 30, 0, DateTimeKind.Unspecified), "AMBIGUOUS")
};
int failed = 0;
foreach (var c in cases) {
    var result = AppointmentScheduler.Schedule(c.Local, zone);
    Console.WriteLine($"{c.Name}: actual={result.Status}; expected={c.Expected}; utc={result.Utc:o}");
    if (result.Status != c.Expected) failed++;
}
Console.WriteLine($"Contract failures: {failed}");
return failed == 0 ? 0 : 1;

public readonly record struct ScheduleResult(string Status, DateTime? Utc);
public static class AppointmentScheduler {
    public static ScheduleResult Schedule(DateTime local, TimeZoneInfo zone) {
        try {
            DateTime utc = TimeZoneInfo.ConvertTimeToUtc(local, zone);
            return new ScheduleResult("ACCEPT", utc);
        } catch (ArgumentException) {
            return new ScheduleResult("INVALID", null);
        }
    }
}
