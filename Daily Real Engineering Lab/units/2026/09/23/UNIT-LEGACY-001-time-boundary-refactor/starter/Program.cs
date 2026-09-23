var incidentInstant = new DateTimeOffset(2026, 9, 23, 10, 0, 0, TimeSpan.Zero);
var subscription = new Subscription("SUB-1042", incidentInstant.AddMinutes(1));
var service = new RenewalEligibilityService();

Console.WriteLine($"Incident instant : {incidentInstant:O}");
Console.WriteLine($"Expires at       : {subscription.ExpiresAt:O}");
Console.WriteLine($"Current runtime  : {DateTimeOffset.UtcNow:O}");
Console.WriteLine($"Expired?         : {service.IsExpired(subscription)}");

if (service.IsExpired(subscription))
{
    Console.WriteLine("REPRODUCED: replay cannot evaluate the rule at the recorded incident instant.");
    Environment.ExitCode = 1;
}

public sealed record Subscription(string Id, DateTimeOffset ExpiresAt);

public sealed class RenewalEligibilityService
{
    public bool IsExpired(Subscription subscription)
    {
        // Investigation note: list every input that can affect this decision.
        return DateTimeOffset.UtcNow >= subscription.ExpiresAt;
    }
}
