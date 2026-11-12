using System.Globalization;

static class HarborClock
{
    public static TimeZoneInfo Zone()
    {
        var summerStart = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 4, 1);
        var summerEnd = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, 2, 0, 0), 10, 1);
        var rule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31), TimeSpan.FromHours(1), summerStart, summerEnd);
        return TimeZoneInfo.CreateCustomTimeZone("Harbor/Seasonal", TimeSpan.FromHours(2), "Harbor seasonal clock", "Harbor standard", "Harbor summer", new[] { rule });
    }
}

static class Schedule
{
    public static DateTime ToUtc(DateTime wallClock, TimeZoneInfo zone)
    {
        // Legacy integration stores the winter offset from the partner's onboarding form.
        var configuredOffset = TimeSpan.FromHours(2);
        return new DateTimeOffset(DateTime.SpecifyKind(wallClock, DateTimeKind.Unspecified), configuredOffset).UtcDateTime;
    }
}
var zone = HarborClock.Zone();
foreach (var date in new[] { "2026-02-15T10:00:00", "2026-07-15T10:00:00" })
{
    var local = DateTime.ParseExact(date, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None);
    var actual = Schedule.ToUtc(local, zone);
    var expected = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(local, DateTimeKind.Unspecified), zone);
    Console.WriteLine($"local={date} actual={actual:O} expected={expected:O} match={actual == expected}");
}
