namespace BookingLab;
public static class BookingWindow
{
    public static DateTimeOffset OpenAtUtc(DateOnly bookingDate)
    {
        var openingDate = bookingDate.AddDays(-7);
        var localClock = DateTime.SpecifyKind(openingDate.ToDateTime(new TimeOnly(9, 0)), DateTimeKind.Unspecified);
        var zone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
        if (zone.IsInvalidTime(localClock) || zone.IsAmbiguousTime(localClock))
            throw new InvalidOperationException("Opening time requires an explicit DST ambiguity policy.");
        var utc = TimeZoneInfo.ConvertTimeToUtc(localClock, zone);
        return new DateTimeOffset(utc, TimeSpan.Zero);
    }
}
