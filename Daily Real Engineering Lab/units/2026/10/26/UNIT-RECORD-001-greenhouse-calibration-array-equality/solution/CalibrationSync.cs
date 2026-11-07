namespace GreenhouseLab;

public sealed record CalibrationProfile(string ZoneId, int Revision, int[] MoistureThresholds);

public sealed class CalibrationSync
{
    private sealed record AppliedSnapshot(int Revision, int[] MoistureThresholds);

    private readonly Dictionary<string, AppliedSnapshot> _lastApplied =
        new(StringComparer.Ordinal);

    public List<string> AppliedZones { get; } = new();

    public void Observe(CalibrationProfile incoming)
    {
        ArgumentNullException.ThrowIfNull(incoming);
        ArgumentNullException.ThrowIfNull(incoming.MoistureThresholds);

        if (_lastApplied.TryGetValue(incoming.ZoneId, out var previous) &&
            previous.Revision == incoming.Revision &&
            previous.MoistureThresholds.AsSpan().SequenceEqual(incoming.MoistureThresholds))
        {
            return;
        }

        AppliedZones.Add(incoming.ZoneId);
        // The cached state is a snapshot, not an alias of caller-owned mutable memory.
        _lastApplied[incoming.ZoneId] = new(
            incoming.Revision,
            (int[])incoming.MoistureThresholds.Clone());
    }
}
