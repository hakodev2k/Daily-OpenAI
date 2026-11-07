using VisitorPassLab;

var cutoff = new DateTimeOffset(2026, 11, 1, 15, 0, 0, TimeSpan.Zero);
var cases = new[]
{
    new ContractCase("active-before-expiry", cutoff.AddSeconds(-1), cutoff, null, false, true, true),
    new ContractCase("expires-at-boundary", cutoff, cutoff, null, false, true, false),
    new ContractCase("expired-but-present", cutoff.AddSeconds(30), cutoff, null, false, true, false),
    new ContractCase("revoked-but-present", cutoff.AddMinutes(-5), cutoff.AddHours(1), cutoff.AddMinutes(-10), false, true, false),
    new ContractCase("cleanup-eventually-removes", cutoff.AddMinutes(2), cutoff, null, true, true, false),
    new ContractCase("missing-document", cutoff, cutoff.AddHours(1), null, false, false, false),
    new ContractCase("future-revocation-not-yet-effective", cutoff, cutoff.AddHours(1), cutoff.AddMinutes(10), false, true, true)
};

var results = cases.Select(Evaluate).ToArray();
foreach (var result in results)
{
    Console.WriteLine($"{result.Name}: actual={(result.Actual ? "ALLOW" : "DENY")}, expected={(result.Expected ? "ALLOW" : "DENY")} => {(result.Passed ? "PASS" : "FAIL")}");
}

var mode = args.Length == 0 ? "--run" : args[0];
if (mode == "--reproduce")
{
    var requiredSymptoms = new[] { "expires-at-boundary", "expired-but-present", "revoked-but-present" };
    bool reproduced = requiredSymptoms.All(name =>
        results.Any(result => result.Name == name && !result.Passed));
    Console.WriteLine(reproduced ? "REPRODUCED: access contract violated." : "NOT REPRODUCED: expected starter symptoms absent.");
    return reproduced ? 0 : 1;
}

if (mode == "--verify")
{
    bool allPassed = results.All(result => result.Passed);
    Console.WriteLine(allPassed ? "VERIFIED: all authorization contracts pass." : "FAILED: authorization contract regressions remain.");
    return allPassed ? 0 : 1;
}

if (mode == "--run") return 0;
Console.Error.WriteLine("Usage: --run | --reproduce | --verify");
return 2;

static ContractResult Evaluate(ContractCase test)
{
    var store = new PassStore();
    if (test.DocumentExists)
        store.Upsert(new AccessPass("GUEST-72", test.ExpiresAtUtc, test.RevokedAtUtc));
    if (test.RunCleanup)
        store.RunCleanup(test.NowUtc);
    bool actual = AccessPolicy.CanConnect(store, "GUEST-72", test.NowUtc);
    return new ContractResult(test.Name, actual, test.Expected);
}

internal sealed record ContractCase(
    string Name,
    DateTimeOffset NowUtc,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset? RevokedAtUtc,
    bool RunCleanup,
    bool DocumentExists,
    bool Expected);

internal sealed record ContractResult(string Name, bool Actual, bool Expected)
{
    public bool Passed => Actual == Expected;
}
