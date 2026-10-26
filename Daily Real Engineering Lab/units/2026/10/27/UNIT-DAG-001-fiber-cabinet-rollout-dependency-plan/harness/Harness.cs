using System;
using System.Collections.Generic;
using System.Linq;

public sealed record RolloutStep(string Id, string[] Prerequisites);
public sealed record PlanResult(bool Success, IReadOnlyList<string> Order, string? Error);

public static class Harness
{
    public static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "demo" : args[0];
        return mode switch
        {
            "demo" => Demo(),
            "reproduce" => Reproduce(),
            "verify" => Verify(),
            _ => UnknownMode(mode)
        };
    }

    private static int UnknownMode(string mode)
    {
        Console.Error.WriteLine("Unknown mode: " + mode);
        return 2;
    }

    private static RolloutStep[] Valid() => new[]
    {
        new RolloutStep("download-image", Array.Empty<string>()),
        new RolloutStep("verify-signature", new[] { "download-image" }),
        new RolloutStep("backup-config", new[] { "verify-signature" }),
        new RolloutStep("flash-ring-a", new[] { "backup-config" }),
        new RolloutStep("healthcheck-ring-a", new[] { "flash-ring-a" }),
        new RolloutStep("announce-completion", new[] { "healthcheck-ring-a" })
    };

    private static RolloutStep[] Incident()
    {
        RolloutStep[] steps = Valid();
        steps[3] = new RolloutStep("flash-ring-a", new[] { "backup-config", "healthcheck-ring-a" });
        return steps;
    }

    private static RolloutStep[] UnknownPrerequisite()
    {
        RolloutStep[] steps = Valid();
        steps[3] = new RolloutStep("flash-ring-a", new[] { "backup-config", "approval-not-in-manifest" });
        return steps;
    }

    private static RolloutStep[] DuplicateIds()
    {
        var steps = Valid().ToList();
        steps.Add(new RolloutStep("download-image", Array.Empty<string>()));
        return steps.ToArray();
    }

    private static RolloutStep[] SelfReference() => new[]
    {
        new RolloutStep("healthcheck-ring-a", new[] { "healthcheck-ring-a" })
    };

    private static void Show(PlanResult plan, int declared)
    {
        Console.WriteLine("status=" + (plan.Success ? "SUCCESS" : "REJECTED"));
        Console.WriteLine("planned=" + plan.Order.Count + "/" + declared);
        Console.WriteLine("steps=" + string.Join(",", plan.Order));
        Console.WriteLine("diagnostic=" + (plan.Error ?? "<none>"));
    }

    private static int Demo()
    {
        RolloutStep[] steps = Incident();
        Console.WriteLine("Incident: telecom firmware ring A");
        Show(Planner.Plan(steps), steps.Length);
        return 0;
    }

    private static int Reproduce()
    {
        RolloutStep[] steps = Incident();
        PlanResult plan = Planner.Plan(steps);
        Show(plan, steps.Length);
        bool symptom = plan.Success && plan.Order.Count < steps.Length;
        Console.WriteLine(symptom ? "REPRODUCED: successful incomplete plan" : "NOT REPRODUCED");
        return symptom ? 0 : 1;
    }

    private static bool ValidGraph()
    {
        RolloutStep[] steps = Valid();
        PlanResult result = Planner.Plan(steps);
        if (!result.Success || result.Error != null || result.Order.Count != steps.Length)
            return false;
        var positions = result.Order.Select((id, index) => new { id, index })
            .ToDictionary(x => x.id, x => x.index, StringComparer.Ordinal);
        return positions.Count == steps.Length &&
            steps.All(s => positions.ContainsKey(s.Id) &&
                s.Prerequisites.All(p => positions.ContainsKey(p) && positions[p] < positions[s.Id]));
    }

    private static bool CycleRejected()
    {
        PlanResult result = Planner.Plan(Incident());
        return !result.Success && result.Order.Count == 0 &&
            result.Error?.Contains("cycle", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool UnknownRejected()
    {
        PlanResult result = Planner.Plan(UnknownPrerequisite());
        return !result.Success && result.Order.Count == 0 &&
            result.Error?.Contains("unknown", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool DuplicateRejected()
    {
        PlanResult result = Planner.Plan(DuplicateIds());
        return !result.Success && result.Order.Count == 0 &&
            result.Error?.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool SelfCycleRejected()
    {
        PlanResult result = Planner.Plan(SelfReference());
        return !result.Success && result.Order.Count == 0 &&
            result.Error?.Contains("cycle", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static bool EmptyGraph()
    {
        PlanResult result = Planner.Plan(Array.Empty<RolloutStep>());
        return result.Success && result.Order.Count == 0 && result.Error == null;
    }

    private static bool StablePlan()
    {
        PlanResult first = Planner.Plan(Valid());
        PlanResult second = Planner.Plan(Valid());
        return first.Success && second.Success && first.Order.SequenceEqual(second.Order);
    }

    private static int Verify()
    {
        var checks = new (string Name, Func<bool> Run)[]
        {
            ("valid prerequisites and completeness", ValidGraph),
            ("incident cycle rejected without partial plan", CycleRejected),
            ("unknown prerequisite diagnosed", UnknownRejected),
            ("duplicate IDs diagnosed", DuplicateRejected),
            ("self dependency rejected", SelfCycleRejected),
            ("empty rollout is valid", EmptyGraph),
            ("repeatable plan order", StablePlan)
        };
        int failed = 0;
        foreach (var check in checks)
        {
            bool passed;
            try { passed = check.Run(); }
            catch (Exception ex)
            {
                passed = false;
                Console.WriteLine("EXCEPTION " + check.Name + ": " + ex.GetType().Name);
            }
            Console.WriteLine((passed ? "PASS " : "FAIL ") + check.Name);
            if (!passed) failed++;
        }
        Console.WriteLine("summary=" + (checks.Length - failed) + "/" + checks.Length);
        return failed == 0 ? 0 : 1;
    }
}
