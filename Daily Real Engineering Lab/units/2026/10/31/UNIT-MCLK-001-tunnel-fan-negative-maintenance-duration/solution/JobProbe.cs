namespace ClockLab;

public sealed record MaintenanceMeasurement(
    DateTimeOffset StartedAtUtc,
    DateTimeOffset FinishedAtUtc,
    TimeSpan Duration);

public static class MaintenanceProbe
{
    public static async Task<MaintenanceMeasurement> MeasureAsync(
        TimeProvider clock, Func<Task> inspect)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(inspect);

        var startedAt = clock.GetUtcNow();
        var startedTimestamp = clock.GetTimestamp();

        await inspect();

        var finishedTimestamp = clock.GetTimestamp();
        var finishedAt = clock.GetUtcNow();
        var elapsed = clock.GetElapsedTime(startedTimestamp, finishedTimestamp);

        return new MaintenanceMeasurement(startedAt, finishedAt, elapsed);
    }
}
