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
        await inspect();
        var finishedAt = clock.GetUtcNow();

        return new MaintenanceMeasurement(
            startedAt, finishedAt, finishedAt - startedAt);
    }
}
