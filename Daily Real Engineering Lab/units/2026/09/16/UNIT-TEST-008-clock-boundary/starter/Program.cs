var expiry = DateTimeOffset.Parse("2026-09-16T06:30:00+07:00");
var service = new EntitlementService();

var before = service.IsActive(expiry, DateTimeOffset.Parse("2026-09-16T06:29:59+07:00"));
var atBoundary = service.IsActive(expiry, DateTimeOffset.Parse("2026-09-16T06:30:00+07:00"));

Console.WriteLine($"Before boundary: {before}");
Console.WriteLine($"At boundary: {atBoundary}");

public sealed class EntitlementService
{
    public bool IsActive(DateTimeOffset expiresAt, DateTimeOffset observationTime)
    {
        // Investigation note: compare the dependency used by the rule with the
        // observation time controlled by the caller/harness.
        var current = DateTimeOffset.Now;
        return current < expiresAt;
    }
}