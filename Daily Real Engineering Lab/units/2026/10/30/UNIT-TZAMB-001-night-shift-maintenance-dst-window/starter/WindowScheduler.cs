namespace MaintenanceScheduling;

public sealed record WindowRequest(string Id, DateTime LocalStart, int? UtcOffsetMinutes);

public static class WindowScheduler
{
    public static DateTimeOffset Resolve(WindowRequest request)
    {
        var zone = LoadPartnerZone();
        var local = DateTime.SpecifyKind(request.LocalStart, DateTimeKind.Unspecified);
        var utc = TimeZoneInfo.ConvertTimeToUtc(local, zone);
        return new DateTimeOffset(utc, TimeSpan.Zero);
    }

    private static TimeZoneInfo LoadPartnerZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }
    }
}
