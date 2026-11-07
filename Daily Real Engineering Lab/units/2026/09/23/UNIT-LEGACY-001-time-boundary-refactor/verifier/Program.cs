var expiration = new DateTimeOffset(2026, 9, 23, 10, 1, 0, TimeSpan.Zero);
var subscription = new Subscription("SUB-1042", expiration);

Assert(false, expiration.AddTicks(-1));
Assert(true, expiration);
Assert(true, expiration.AddTicks(1));
Console.WriteLine("PASS: boundary behavior is deterministic and replayable.");

void Assert(bool expected, DateTimeOffset now)
{
    var service = new RenewalEligibilityService(new FixedTimeProvider(now));
    var actual = service.IsExpired(subscription);
    if (actual != expected) throw new Exception($"Expected {expected} at {now:O}, got {actual}.");
}

sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}
