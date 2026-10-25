using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using TenantQuotaLab;

namespace TenantQuotaHarness;

internal static class Program
{
    private sealed record Step(string Name, string? Tenant, string Path, bool Expected);
    private sealed record Scenario(string Id, string Description, Step[] Steps);

    private static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "--run" : args[0];
        if (mode is not ("--run" or "--reproduce" or "--verify"))
        {
            Console.Error.WriteLine("Usage: --run | --reproduce | --verify");
            return 2;
        }

        int failedScenarios = 0;
        bool controlPassed = false;

        foreach (Scenario scenario in Scenarios())
        {
            using PartitionedRateLimiter<HttpContext> limiter = TenantQuotaPolicy.Create();
            bool allMatched = true;
            Console.WriteLine($"SCENARIO {scenario.Id}: {scenario.Description}");

            foreach (Step step in scenario.Steps)
            {
                HttpContext context = Context(step.Tenant, step.Path);
                using RateLimitLease lease = limiter.AttemptAcquire(context);
                bool actual = lease.IsAcquired;
                bool matched = actual == step.Expected;
                allMatched &= matched;
                Console.WriteLine(
                    $"  {step.Name,-18} tenant={step.Tenant ?? "(anonymous)",-12} " +
                    $"path={step.Path,-20} expected={(step.Expected ? "ALLOW" : "DENY"),-5} " +
                    $"actual={(actual ? "ALLOW" : "DENY"),-5} {(matched ? "OK" : "MISMATCH")}");
            }

            if (!allMatched) failedScenarios++;
            if (scenario.Id == "D") controlPassed = allMatched;
        }

        Console.WriteLine($"SUMMARY failed_scenarios={failedScenarios} control_passed={controlPassed}");

        if (mode == "--run") return 0;

        if (mode == "--reproduce")
        {
            if (failedScenarios == 3 && controlPassed)
            {
                Console.WriteLine("EXPECTED_INCIDENT_REPRODUCED");
                return 0;
            }

            Console.Error.WriteLine("REPRODUCTION_NOT_MATCHING_BASELINE");
            return 1;
        }

        if (failedScenarios == 0)
        {
            Console.WriteLine("VERIFICATION_PASSED");
            return 0;
        }

        Console.Error.WriteLine("VERIFICATION_FAILED");
        return 1;
    }

    private static HttpContext Context(string? tenantId, string path)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;

        if (tenantId is not null)
        {
            context.User = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim("tenant_id", tenantId) }, "lab-auth"));
        }

        return context;
    }

    private static IEnumerable<Scenario> Scenarios()
    {
        yield return new Scenario("A", "Two authenticated tenants on one endpoint", new[]
        {
            new Step("A first",  "alpha", "/api/render", true),
            new Step("A second", "alpha", "/api/render", true),
            new Step("B first",  "beta",  "/api/render", true)
        });

        yield return new Scenario("B", "One tenant across two endpoints", new[]
        {
            new Step("A render",  "alpha", "/api/render", true),
            new Step("A status",  "alpha", "/api/status", true),
            new Step("A third",   "alpha", "/api/render", false)
        });

        yield return new Scenario("C", "Unauthenticated requests share a bounded fallback", new[]
        {
            new Step("anon render", null, "/api/render", true),
            new Step("anon status", null, "/api/status", true),
            new Step("anon third",  null, "/api/render", false)
        });

        yield return new Scenario("D", "Positive control: one tenant exhausts its budget", new[]
        {
            new Step("A first",  "alpha", "/api/render", true),
            new Step("A second", "alpha", "/api/render", true),
            new Step("A third",  "alpha", "/api/render", false)
        });
    }
}
