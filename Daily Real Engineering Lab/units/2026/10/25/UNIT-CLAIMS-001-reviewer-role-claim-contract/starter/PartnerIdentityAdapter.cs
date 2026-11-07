using System.Security.Claims;

namespace RoleClaimLab;

public sealed record PartnerIdentityTicket(string Subject, string[] Roles);

public static class PartnerIdentityAdapter
{
    // This adapter receives a normalized ticket AFTER issuer, signature,
    // audience, lifetime and trust checks have already succeeded.
    public static ClaimsPrincipal FromValidatedTicket(PartnerIdentityTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        var claims = new List<Claim> { new("sub", ticket.Subject) };
        claims.AddRange(ticket.Roles.Select(role => new Claim("roles", role)));

        return new ClaimsPrincipal(
            new ClaimsIdentity(claims, authenticationType: "Bearer"));
    }
}
