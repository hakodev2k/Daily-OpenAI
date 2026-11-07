static DateTimeOffset ScheduleAtNewYorkNine(DateOnly date)
{
    // BUG: team assumed New York is always UTC-05:00.
    var wallClock = date.ToDateTime(new TimeOnly(9, 0), DateTimeKind.Unspecified);
    return new DateTimeOffset(wallClock, TimeSpan.FromHours(-5)).ToUniversalTime();
}

var beforeDst = ScheduleAtNewYorkNine(new DateOnly(2026, 3, 7));
var afterDst = ScheduleAtNewYorkNine(new DateOnly(2026, 3, 9));

Console.WriteLine($"BEFORE_DST={beforeDst:yyyy-MM-ddTHH:mm:ss'Z'}");
Console.WriteLine($"AFTER_DST={afterDst:yyyy-MM-ddTHH:mm:ss'Z'}");
Console.WriteLine("EXPECTED_BEFORE_DST=2026-03-07T14:00:00Z");
Console.WriteLine("EXPECTED_AFTER_DST=2026-03-09T13:00:00Z");
