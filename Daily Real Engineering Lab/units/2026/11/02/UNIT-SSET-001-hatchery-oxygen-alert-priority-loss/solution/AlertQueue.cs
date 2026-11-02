using System.Collections.Generic;
using System.Linq;
namespace HatcheryLab;
public sealed record OxygenAlert(string AlertId, int Severity, DateTimeOffset RaisedAt, string TankId);
public sealed class AlertQueue
{
    private static readonly IComparer<OxygenAlert> AlertOrder =
        Comparer<OxygenAlert>.Create((left, right) =>
        {
            int severity = right.Severity.CompareTo(left.Severity);
            if (severity != 0) return severity;
            int raisedAt = left.RaisedAt.CompareTo(right.RaisedAt);
            if (raisedAt != 0) return raisedAt;
            return StringComparer.Ordinal.Compare(left.AlertId, right.AlertId);
        });
    private readonly SortedSet<OxygenAlert> _pending = new(AlertOrder);
    public bool Enqueue(OxygenAlert alert) => _pending.Add(alert);
    public IReadOnlyList<OxygenAlert> Snapshot() => _pending.ToArray();
}
