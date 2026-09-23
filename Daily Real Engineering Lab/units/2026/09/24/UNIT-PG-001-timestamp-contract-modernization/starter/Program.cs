var deliveryOffset = TimeSpan.FromHours(7);
var scheduledLocal = new DateTimeOffset(2026, 9, 24, 9, 30, 0, deliveryOffset);

Console.WriteLine($"Original: {scheduledLocal:O}");
Console.WriteLine($"Original UTC: {scheduledLocal.UtcDateTime:O}");

var stored = LegacyPersistence.Write(scheduledLocal);
var loaded = LegacyPersistence.Read(stored);

Console.WriteLine($"Stored: {stored:O} Kind={stored.Kind}");
Console.WriteLine($"Loaded: {loaded:O}");
Console.WriteLine($"Loaded UTC: {loaded.UtcDateTime:O}");

if (loaded.UtcDateTime != scheduledLocal.UtcDateTime)
{
    Console.Error.WriteLine("FAIL: round-trip changed the business instant.");
    Environment.ExitCode = 2;
    return;
}

Console.WriteLine("PASS: round-trip preserved the business instant.");

static class LegacyPersistence
{
    public static DateTime Write(DateTimeOffset value)
    {
        // Legacy column contract stores only a wall-clock DateTime.
        return value.DateTime;
    }

    public static DateTimeOffset Read(DateTime stored)
    {
        // The newer service treats the persisted value as UTC.
        var interpretedUtc = DateTime.SpecifyKind(stored, DateTimeKind.Utc);
        return new DateTimeOffset(interpretedUtc);
    }
}