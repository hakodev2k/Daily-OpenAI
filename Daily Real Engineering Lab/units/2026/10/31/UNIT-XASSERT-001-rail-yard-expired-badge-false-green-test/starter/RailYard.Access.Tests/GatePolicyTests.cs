using RailYard.Access;
using Xunit;

namespace RailYard.Access.Tests;

public sealed class GatePolicyTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 31, 7, 0, 0, TimeSpan.Zero);
    private readonly GatePolicy _policy = new();

    [Fact]
    public void Expired_badge_must_be_rejected()
    {
        var pass = new AccessPass("CONTRACTOR-17", Now.AddMinutes(-1), Revoked: false);
        Assert.ThrowsAsync<AccessDeniedException>(() => _policy.ValidateAsync(pass, Now));
    }

    [Fact]
    public async Task Revoked_badge_must_be_rejected()
    {
        var pass = new AccessPass("CONTRACTOR-18", Now.AddHours(1), Revoked: true);
        await Assert.ThrowsAsync<AccessDeniedException>(() => _policy.ValidateAsync(pass, Now));
    }

    [Fact]
    public async Task Valid_badge_must_be_accepted()
    {
        var pass = new AccessPass("CONTRACTOR-19", Now.AddHours(1), Revoked: false);
        await _policy.ValidateAsync(pass, Now);
    }
}
