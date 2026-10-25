namespace ReminderScheduler;

public static class ScheduleBuilder
{
    private static readonly TimeOnly InterviewTime = new(9, 0);

    public static IReadOnlyList<Reminder> Build(DateOnly firstDate, int days)
    {
        if (days is < 1 or > 31) throw new ArgumentOutOfRangeException(nameof(days));

        var zone = FindInterviewZone();
        var reminders = new List<Reminder>(days);

        for (var i = 0; i < days; i++)
        {
            var date = firstDate.AddDays(i);
            var local = date.ToDateTime(InterviewTime, DateTimeKind.Unspecified);

            if (zone.IsInvalidTime(local) || zone.IsAmbiguousTime(local))
                throw new InvalidOperationException(
                    $"Local interview time requires explicit scheduling policy: {local:O}");

            var utc = TimeZoneInfo.ConvertTimeToUtc(local, zone);
            reminders.Add(new Reminder(date, utc));
        }

        return reminders;
    }

    private static TimeZoneInfo FindInterviewZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("America/New_York"); }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }
    }
}
