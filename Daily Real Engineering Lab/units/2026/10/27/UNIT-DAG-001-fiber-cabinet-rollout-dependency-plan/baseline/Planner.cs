using System;
using System.Collections.Generic;
using System.Linq;

public static class Planner
{
    public static PlanResult Plan(IReadOnlyList<RolloutStep> steps)
    {
        var pending = steps.ToDictionary(s => s.Id, s => s, StringComparer.Ordinal);
        var completed = new HashSet<string>(StringComparer.Ordinal);
        var order = new List<string>();

        while (pending.Count > 0)
        {
            var ready = pending.Values
                .Where(s => s.Prerequisites.All(completed.Contains))
                .OrderBy(s => s.Id, StringComparer.Ordinal)
                .ToArray();

            if (ready.Length == 0)
                break;

            foreach (RolloutStep step in ready)
            {
                order.Add(step.Id);
                completed.Add(step.Id);
                pending.Remove(step.Id);
            }
        }

        return new PlanResult(true, order, null);
    }
}
