namespace GreenhouseLab;

public sealed record CalibrationProfile(string ZoneId, int Revision, int[] MoistureThresholds);

public sealed class CalibrationSync
{
    private readonly Dictionary<string, CalibrationProfile> _lastApplied =
        new(StringComparer.Ordinal);

    // In production, each entry represents one configuration push to a controller.
    public List<string> AppliedZones { get; } = new();

    public void Observe(CalibrationProfile incoming)
    {
        ArgumentNullException.ThrowIfNull(incoming);

        if (_lastApplied.TryGetValue(incoming.ZoneId, out var previous) &&
            previous == incoming)
        {
            return;
        }

        AppliedZones.Add(incoming.ZoneId);
        _lastApplied[incoming.ZoneId] = incoming;
    }
}
