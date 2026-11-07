using System.Collections.Generic;
using System.Linq;
namespace HatcheryLab;
public sealed record OxygenAlert(string AlertId, int Severity, DateTimeOffset RaisedAt, string TankId);
public sealed class AlertQueue
{
    private readonly SortedSet<OxygenAlert> _pending =
        new(Comparer<OxygenAlert>.Create((left, right) =>
            right.Severity.CompareTo(left.Severity)));
    public bool Enqueue(OxygenAlert alert) => _pending.Add(alert);
    public IReadOnlyList<OxygenAlert> Snapshot() => _pending.ToArray();
}
