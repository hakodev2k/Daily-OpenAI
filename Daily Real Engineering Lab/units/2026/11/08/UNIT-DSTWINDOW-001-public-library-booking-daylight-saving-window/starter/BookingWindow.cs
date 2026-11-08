namespace BookingLab;
public static class BookingWindow
{
    // API: target local booking date; opens seven calendar days before at 09:00 in New York.
    public static DateTimeOffset OpenAtUtc(DateOnly bookingDate)
    {
        var openingDate = bookingDate.AddDays(-7);
        var localClock = openingDate.ToDateTime(new TimeOnly(9, 0));
        var fixedOffset = TimeSpan.FromHours(-5);
        return new DateTimeOffset(localClock, fixedOffset).ToUniversalTime();
    }
}
