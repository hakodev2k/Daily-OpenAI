using System.Security.Claims;
using RoleClaimLab;

var mode = args.Length == 1 ? args[0].ToLowerInvariant() : "verify";
if (mode is not ("reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project checks/ContractChecks.csproj -- reproduce|verify");
    return 2;
}

var cases = new (string Name, ClaimsPrincipal Principal, bool Expected)[]
{
    ("reviewer", PartnerIdentityAdapter.FromValidatedTicket(new("r-104", ["Reviewer"])), true),
    ("reviewer-with-auditor", PartnerIdentityAdapter.FromValidatedTicket(new("r-310", ["Auditor", "Reviewer"])), true),
    ("reader", PartnerIdentityAdapter.FromValidatedTicket(new("r-208", ["Reader"])), false),
    ("no-role", PartnerIdentityAdapter.FromValidatedTicket(new("r-401", [])), false),
    ("near-match", PartnerIdentityAdapter.FromValidatedTicket(new("r-402", ["reviewer"])), false),
    ("unauthenticated", new ClaimsPrincipal(new ClaimsIdentity([new Claim("roles", "Reviewer")])), false)
};

var observed = new List<(string Name, bool Actual, bool Expected)>();
foreach (var test in cases)
{
    var actual = await ReviewAuthorization.IsReviewerAsync(test.Principal);
    observed.Add((test.Name, actual, test.Expected));
    Console.WriteLine($"{test.Name}: authorized={actual}; expected={test.Expected}");
}

var positive = observed.Where(x => x.Expected).ToArray();
var negative = observed.Where(x => !x.Expected).ToArray();
var negativesDenied = negative.All(x => !x.Actual);
if (mode == "reproduce")
{
    // This script confirms the original, intentionally incorrect starter behavior.
    var reproduced = positive.All(x => !x.Actual) && negativesDenied;
    Console.WriteLine(reproduced
        ? "REPRODUCED: valid reviewer cases denied; negative guardrails remain denied."
        : "NOT REPRODUCED: starter no longer exhibits the expected symptom.");
    return reproduced ? 0 : 1;
}

var verified = positive.All(x => x.Actual) && negativesDenied;
Console.WriteLine(verified
    ? "VERIFIED: reviewer access restored; all negative guardrails remain denied."
    : "VERIFY FAILED: expected access contract not satisfied.");
return verified ? 0 : 1;
