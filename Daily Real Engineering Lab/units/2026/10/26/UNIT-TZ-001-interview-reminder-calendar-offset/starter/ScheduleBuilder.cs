namespace ReminderScheduler;

public static class ScheduleBuilder
{
    private static readonly TimeOnly InterviewTime = new(9, 0);

    public static IReadOnlyList<Reminder> Build(DateOnly firstDate, int days)
    {
        if (days is < 1 or > 31) throw new ArgumentOutOfRangeException(nameof(days));

        var zone = FindInterviewZone();
        var firstLocal = firstDate.ToDateTime(InterviewTime, DateTimeKind.Unspecified);
        var batchOffset = zone.GetUtcOffset(firstLocal);
        var reminders = new List<Reminder>(days);

        for (var i = 0; i < days; i++)
        {
            var date = firstDate.AddDays(i);
            var local = date.ToDateTime(InterviewTime, DateTimeKind.Unspecified);
            var utc = new DateTimeOffset(local, batchOffset).UtcDateTime;
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
