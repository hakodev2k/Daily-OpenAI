using FleetRoutingLab;

internal static class Program
{
    private const string Route = "fleet-north";

    public static int Main(string[] args)
    {
        var mode = args.Length == 0 ? "demo" : args[0];
        return mode switch
        {
            "--demo" => Demo(),
            "--reproduce" => Reproduce(),
            "--verify" => Verify(),
            _ => Unknown(mode)
        };
    }

    private static int Unknown(string mode)
    {
        Console.Error.WriteLine($"Unknown mode: {mode}");
        return 2;
    }

    private static (PlanStore Store, LeaseToken A, LeaseToken B, bool StaleAccepted) RunIncident()
    {
        var clock = new ManualClock();
        var leases = new LeaseRegistry(clock);
        var store = new PlanStore();

        var a = leases.Acquire(Route, "worker-A", 10);
        clock.Advance(11);
        var b = leases.Acquire(Route, "worker-B", 10);
        store.TryCommit(b, "plan-from-B");
        bool staleAccepted = store.TryCommit(a, "plan-from-A");
        return (store, a, b, staleAccepted);
    }

    private static int Demo()
    {
        var incident = RunIncident();
        var saved = incident.Store.Read(Route);
        Console.WriteLine($"A epoch={incident.A.Epoch}, B epoch={incident.B.Epoch}");
        Console.WriteLine($"Delayed A accepted={incident.StaleAccepted}");
        Console.WriteLine($"Final plan={saved?.PlanId}; stored epoch={saved?.LeaseEpoch}");
        return 0;
    }

    private static int Reproduce()
    {
        var incident = RunIncident();
        var saved = incident.Store.Read(Route);
        if (incident.StaleAccepted && saved?.PlanId == "plan-from-A")
        {
            Console.WriteLine("EXPECTED_INCIDENT_REPRODUCED");
            Console.WriteLine("A delayed writer replaced the newer committed route plan.");
            return 0;
        }
        Console.Error.WriteLine("INCIDENT_NOT_REPRODUCED: starter may already have been fixed.");
        return 1;
    }

    private static int Verify()
    {
        int failures = 0;
        void Check(string label, bool condition)
        {
            Console.WriteLine($"{(condition ? "PASS" : "FAIL")}: {label}");
            if (!condition) failures++;
        }

        var incident = RunIncident();
        var final = incident.Store.Read(Route);
        Check("newer owner remains authoritative", !incident.StaleAccepted
            && final?.PlanId == "plan-from-B" && final.LeaseEpoch == incident.B.Epoch);

        var clock = new ManualClock();
        var leases = new LeaseRegistry(clock);
        var store = new PlanStore();
        var first = leases.Acquire("fleet-south", "worker-1", 10);
        Check("first valid plan is accepted", store.TryCommit(first, "south-v1")
            && store.Read("fleet-south")?.PlanId == "south-v1");
        Check("same owner retry is permitted", store.TryCommit(first, "south-v1")
            && store.Read("fleet-south")?.LeaseEpoch == first.Epoch);

        clock.Advance(11);
        var second = leases.Acquire("fleet-south", "worker-2", 10);
        Check("newer plan is accepted", store.TryCommit(second, "south-v2"));
        Check("earlier epoch cannot replace newer epoch", !store.TryCommit(first, "south-late")
            && store.Read("fleet-south")?.PlanId == "south-v2");

        var other = leases.Acquire("fleet-east", "worker-3", 10);
        Check("separate route is unaffected", store.TryCommit(other, "east-v1")
            && store.Read("fleet-east")?.PlanId == "east-v1"
            && store.Read("fleet-south")?.PlanId == "south-v2");

        if (failures == 0)
        {
            Console.WriteLine("ALL_VERIFICATIONS_PASSED");
            return 0;
        }
        Console.Error.WriteLine($"VERIFICATION_FAILED: {failures} assertion(s).");
        return 1;
    }
}
