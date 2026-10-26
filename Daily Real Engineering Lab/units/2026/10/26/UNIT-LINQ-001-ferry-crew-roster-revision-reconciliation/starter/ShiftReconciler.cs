public sealed record CrewDutyUpdate(string CrewId, long Revision, string VesselId, string EventId);
public static class ShiftReconciler
{
    public static IReadOnlyList<CrewDutyUpdate> Reconcile(IEnumerable<CrewDutyUpdate> updates)
    {
        ArgumentNullException.ThrowIfNull(updates);
        return updates
            .DistinctBy(x => x.CrewId, StringComparer.Ordinal)
            .OrderBy(x => x.CrewId, StringComparer.Ordinal)
            .ToArray();
    }
}
