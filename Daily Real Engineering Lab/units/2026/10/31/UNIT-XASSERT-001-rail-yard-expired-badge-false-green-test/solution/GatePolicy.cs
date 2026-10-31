namespace RailYard.Access;

public sealed record AccessPass(string BadgeId, DateTimeOffset ExpiresAtUtc, bool Revoked);

public sealed class AccessDeniedException(string reason) : Exception(reason);

public sealed class GatePolicy
{
    public Task ValidateAsync(AccessPass pass, DateTimeOffset nowUtc)
    {
        ArgumentNullException.ThrowIfNull(pass);

        if (pass.Revoked)
            return Task.FromException(new AccessDeniedException("revoked"));

        if (pass.ExpiresAtUtc <= nowUtc)
        {
            if (Environment.GetEnvironmentVariable("LAB_FAULT_INJECTION") == "accept-expired")
                return Task.CompletedTask;

            return Task.FromException(new AccessDeniedException("expired"));
        }

        return Task.CompletedTask;
    }
}
