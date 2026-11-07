using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;

namespace TenantGateLab;

internal static class Program
{
    private static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "demo" : args[0];
        if (mode is not ("demo" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Mode: demo | reproduce | verify");
            return 2;
        }

        using var limiter = AdmissionPolicy.Create();
        bool a1 = Admit(limiter, "Alpha", "/api/vaccinations", "Alpha", "A1");
        bool a2 = Admit(limiter, "Alpha", "/api/vaccinations", "Alpha", "A2");
        bool b1 = Admit(limiter, "Beta", "/api/vaccinations", "Beta", "B1");
        bool a3 = Admit(limiter, "Alpha", "/api/certificates", "Beta", "A3");
        bool b2 = Admit(limiter, "Beta", "/api/certificates", "Beta", "B2");
        bool b3 = Admit(limiter, "Beta", "/api/vaccinations", "Beta", "B3");

        bool reproduced = a1 && a2 && !b1 && a3;
        bool verified = a1 && a2 && b1 && !a3 && b2 && !b3;

        if (mode == "reproduce")
        {
            Console.WriteLine(reproduced ? "REPRODUCED: cross-clinic and cross-endpoint quota symptoms." :
                "NOT REPRODUCED: original symptom absent.");
            return reproduced ? 0 : 1;
        }

        if (mode == "verify")
        {
            Console.WriteLine(verified ? "PASS: independent clinic quotas, shared endpoint budget, spoofed header ignored." :
                "FAIL: admission violates clinic-level quota contract.");
            return verified ? 0 : 1;
        }

        Console.WriteLine("DEMO ONLY. Reproduce before editing; verify after editing.");
        return 0;
    }

    private static bool Admit(PartitionedRateLimiter<HttpContext> limiter,
        string authenticatedClinic, string path, string suppliedHeader, string requestId)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Request.Headers["X-Clinic-ID"] = suppliedHeader;
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim("clinic_id", authenticatedClinic) }, "fixture-auth"));

        using RateLimitLease lease = limiter.AttemptAcquire(context);
        Console.WriteLine($"{requestId} claim={authenticatedClinic} header={suppliedHeader} path={path} simulatedStatus={(lease.IsAcquired ? 200 : 429)}");
        return lease.IsAcquired;
    }
}
