using ProbeLab;

var policy = new ProbePolicy();
var command = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
if (command == "demo")
{
    var r = ProbeSimulation.Run(policy, true);
    Console.WriteLine($"SUMMARY restarts={r.Restarts} dropped={r.Dropped} fallback={r.Fallback} personalized={r.Personalized}");
    return 0;
}
if (command == "reproduce")
{
    var r = ProbeSimulation.Run(policy, true);
    Console.WriteLine($"SUMMARY restarts={r.Restarts} dropped={r.Dropped} fallback={r.Fallback}");
    if (r.Restarts > 0 && r.Dropped > 0 && r.Fallback == 0)
    {
        Console.WriteLine("REPRODUCED");
        return 0;
    }
    Console.Error.WriteLine("NOT REPRODUCED: original symptom absent.");
    return 1;
}
if (command == "verify")
{
    var ok = new HealthSnapshot(true, true, true);
    var optionalDown = new HealthSnapshot(true, true, false);
    var storeDown = new HealthSnapshot(true, false, true);
    var processDown = new HealthSnapshot(false, true, true);
    var r = ProbeSimulation.Run(policy, false);
    var checks = new (string Name, bool Pass)[]
    {
        ("healthy-live", policy.IsLive(ok)),
        ("healthy-ready", policy.IsReady(ok)),
        ("optional-outage-live", policy.IsLive(optionalDown)),
        ("optional-outage-ready", policy.IsReady(optionalDown)),
        ("optional-outage-fallback", policy.ServeSession(optionalDown) == "200-fallback"),
        ("store-outage-live", policy.IsLive(storeDown)),
        ("store-outage-not-ready", !policy.IsReady(storeDown)),
        ("store-outage-reject", policy.ServeSession(storeDown) == "503"),
        ("process-failure-not-live", !policy.IsLive(processDown)),
        ("process-failure-not-ready", !policy.IsReady(processDown)),
        ("no-restarts", r.Restarts == 0),
        ("no-dropped-requests", r.Dropped == 0),
        ("four-fallbacks", r.Fallback == 4),
        ("four-personalized", r.Personalized == 4)
    };
    foreach (var c in checks) Console.WriteLine($"{(c.Pass ? "PASS" : "FAIL")} {c.Name}");
    int failed = checks.Count(c => !c.Pass);
    Console.WriteLine($"VERIFY {checks.Length - failed}/{checks.Length} passed");
    return failed == 0 ? 0 : 1;
}
Console.Error.WriteLine("Usage: demo|reproduce|verify");
return 2;

internal readonly record struct SimulationResult(int Restarts, int Dropped, int Fallback, int Personalized);
internal static class ProbeSimulation
{
    public static SimulationResult Run(ProbePolicy policy, bool showTimeline)
    {
        int restarts = 0, dropped = 0, fallback = 0, personalized = 0, failures = 0;
        for (int tick = 0; tick < 8; tick++)
        {
            var status = new HealthSnapshot(true, true, tick < 2 || tick > 5);
            bool live = policy.IsLive(status), ready = policy.IsReady(status);
            failures = live ? 0 : failures + 1;
            bool restart = failures >= 2;
            if (restart) { restarts++; failures = 0; }
            string response = ready ? policy.ServeSession(status) : "503-unrouted";
            if (response.StartsWith("503", StringComparison.Ordinal)) dropped++;
            else if (response == "200-fallback") fallback++;
            else if (response == "200-personalized") personalized++;
            if (showTimeline)
                Console.WriteLine($"tick={tick} process={status.ProcessHealthy} store={status.PrimaryStoreReachable} optional={status.OptionalRiskEngineReachable} live={live} ready={ready} response={response} restart={restart}");
        }
        return new SimulationResult(restarts, dropped, fallback, personalized);
    }
}
