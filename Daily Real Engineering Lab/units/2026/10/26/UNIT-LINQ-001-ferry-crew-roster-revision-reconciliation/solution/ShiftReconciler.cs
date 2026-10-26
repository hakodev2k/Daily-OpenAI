/* Reference Solution — only copy to starter after your own attempt. */
public sealed record CrewDutyUpdate(string CrewId, long Revision, string VesselId, string EventId);
public static class ShiftReconciler
{
    public static IReadOnlyList<CrewDutyUpdate> Reconcile(IEnumerable<CrewDutyUpdate> updates)
    {
        ArgumentNullException.ThrowIfNull(updates);
        var snapshot = updates.ToArray();
        return snapshot
            .GroupBy(x => x.CrewId, StringComparer.Ordinal)
            .Select(group =>
            {
                var maxRevision = group.Max(x => x.Revision);
                var candidates = group.Where(x => x.Revision == maxRevision).ToArray();
                var vessel = candidates[0].VesselId;
                if (candidates.Any(x => !StringComparer.Ordinal.Equals(x.VesselId, vessel)))
                    throw new InvalidOperationException("Conflicting assignment for crew " + group.Key);
                return candidates.OrderBy(x => x.EventId, StringComparer.Ordinal).First();
            })
            .OrderBy(x => x.CrewId, StringComparer.Ordinal)
            .ToArray();
    }
}
