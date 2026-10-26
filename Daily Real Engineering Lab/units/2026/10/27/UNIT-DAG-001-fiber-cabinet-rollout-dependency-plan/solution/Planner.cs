using System;
using System.Collections.Generic;
using System.Linq;

public static class Planner
{
    public static PlanResult Plan(IReadOnlyList<RolloutStep> steps)
    {
        var byId = new Dictionary<string, RolloutStep>(StringComparer.Ordinal);
        foreach (RolloutStep step in steps)
        {
            if (string.IsNullOrWhiteSpace(step.Id))
                return Invalid("empty step ID");
            if (!byId.TryAdd(step.Id, step))
                return Invalid("duplicate step ID: " + step.Id);
        }

        var incoming = byId.Keys.ToDictionary(id => id, _ => 0, StringComparer.Ordinal);
        var dependents = byId.Keys.ToDictionary(id => id, _ => new List<string>(), StringComparer.Ordinal);

        foreach (RolloutStep step in byId.Values)
        {
            foreach (string dependency in step.Prerequisites.Distinct(StringComparer.Ordinal))
            {
                if (!byId.ContainsKey(dependency))
                    return Invalid("unknown prerequisite '" + dependency + "' for " + step.Id);
                incoming[step.Id]++;
                dependents[dependency].Add(step.Id);
            }
        }

        var ready = new SortedSet<string>(
            incoming.Where(p => p.Value == 0).Select(p => p.Key),
            StringComparer.Ordinal);
        var order = new List<string>(byId.Count);

        while (ready.Count > 0)
        {
            string id = ready.Min!;
            ready.Remove(id);
            order.Add(id);
            foreach (string dependent in dependents[id])
            {
                incoming[dependent]--;
                if (incoming[dependent] == 0)
                    ready.Add(dependent);
            }
        }

        if (order.Count != byId.Count)
        {
            string unresolved = string.Join(", ", byId.Keys
                .Where(id => incoming[id] > 0)
                .OrderBy(id => id, StringComparer.Ordinal));
            return Invalid("cycle detected; unresolved steps: " + unresolved);
        }

        return new PlanResult(true, order, null);
    }

    private static PlanResult Invalid(string reason) =>
        new PlanResult(false, Array.Empty<string>(), reason);
}
