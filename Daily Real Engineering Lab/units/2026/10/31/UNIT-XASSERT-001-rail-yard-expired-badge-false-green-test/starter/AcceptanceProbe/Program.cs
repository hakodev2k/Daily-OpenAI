using RailYard.Access;

var mode = args.Length == 1 ? args[0] : "verify";
if (mode is not ("reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: AcceptanceProbe [reproduce|verify]");
    return 2;
}

var now = new DateTimeOffset(2026, 10, 31, 7, 0, 0, TimeSpan.Zero);
var policy = new GatePolicy();

static async Task<bool> IsDeniedAsync(GatePolicy policy, AccessPass pass, DateTimeOffset now)
{
    try
    {
        await policy.ValidateAsync(pass, now);
        return false;
    }
    catch (AccessDeniedException)
    {
        return true;
    }
}

var expired = new AccessPass("R-EXPIRED", now.AddSeconds(-1), false);
var revoked = new AccessPass("R-REVOKED", now.AddHours(1), true);
var valid = new AccessPass("R-VALID", now.AddHours(1), false);

var expiredDenied = await IsDeniedAsync(policy, expired, now);
var revokedDenied = await IsDeniedAsync(policy, revoked, now);
var validDenied = await IsDeniedAsync(policy, valid, now);

Console.WriteLine($"expiredDenied={expiredDenied}; revokedDenied={revokedDenied}; validDenied={validDenied}");

if (mode == "reproduce")
{
    if (!expiredDenied && revokedDenied && !validDenied)
    {
        Console.WriteLine("REPRODUCED: expired badge accepted despite green unit tests.");
        return 0;
    }
    Console.Error.WriteLine("Starter symptom not reproduced; inspect modifications or environment.");
    return 1;
}

if (expiredDenied && revokedDenied && !validDenied)
{
    Console.WriteLine("ACCEPTANCE PASS: all three access contracts hold.");
    return 0;
}
Console.Error.WriteLine("ACCEPTANCE FAIL: access policy contract violated.");
return 1;
