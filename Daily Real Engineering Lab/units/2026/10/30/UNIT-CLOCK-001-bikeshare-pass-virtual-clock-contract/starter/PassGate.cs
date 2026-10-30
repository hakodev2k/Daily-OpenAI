namespace BikeShare.Lab;

public enum GateDecision
{
    Allowed,
    NotYetValid,
    Expired,
    InvalidWindow
}

public sealed record UnlockPass(
    string PassId,
    DateTimeOffset ValidFromUtc,
    DateTimeOffset ExpiresAtUtc);

public sealed class PassGate
{
    private readonly TimeProvider _clock;

    public PassGate(TimeProvider clock)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public DateTimeOffset DiagnosticNowUtc() => _clock.GetUtcNow();

    public GateDecision Evaluate(UnlockPass pass)
    {
        ArgumentNullException.ThrowIfNull(pass);
        if (pass.ValidFromUtc >= pass.ExpiresAtUtc)
            return GateDecision.InvalidWindow;

        var now = DateTimeOffset.UtcNow;
        if (now < pass.ValidFromUtc)
            return GateDecision.NotYetValid;

        if (now >= pass.ExpiresAtUtc)
            return GateDecision.Expired;

        return GateDecision.Allowed;
    }
}
