namespace MaintenanceScheduling;

public sealed record WindowRequest(string Id, DateTime LocalStart, int? UtcOffsetMinutes);

public static class WindowScheduler
{
    public static DateTimeOffset Resolve(WindowRequest request)
    {
        if (request.LocalStart.Kind != DateTimeKind.Unspecified)
            throw new ArgumentException("LocalStart must be an unspecified local wall-clock time.");

        var zone = LoadPartnerZone();
        var local = request.LocalStart;

        if (zone.IsInvalidTime(local))
            throw new ArgumentException("The supplied local time does not exist in the partner timezone.");

        TimeSpan selectedOffset;
        if (zone.IsAmbiguousTime(local))
        {
            if (!request.UtcOffsetMinutes.HasValue)
                throw new ArgumentException("Ambiguous local time requires an explicit UTC offset.");

            selectedOffset = TimeSpan.FromMinutes(request.UtcOffsetMinutes.Value);
            var allowedOffsets = zone.GetAmbiguousTimeOffsets(local);
            if (!Array.Exists(allowedOffsets, candidate => candidate == selectedOffset))
                throw new ArgumentException("The supplied UTC offset is not valid for this local time.");
        }
        else
        {
            selectedOffset = zone.GetUtcOffset(local);
            if (request.UtcOffsetMinutes.HasValue &&
                TimeSpan.FromMinutes(request.UtcOffsetMinutes.Value) != selectedOffset)
                throw new ArgumentException("The supplied UTC offset conflicts with the partner timezone.");
        }

        return new DateTimeOffset(local, selectedOffset).ToUniversalTime();
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
