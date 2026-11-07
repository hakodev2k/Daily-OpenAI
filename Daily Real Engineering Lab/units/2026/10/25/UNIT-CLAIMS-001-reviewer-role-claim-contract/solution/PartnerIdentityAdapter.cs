using System.Security.Claims;

namespace RoleClaimLab;

public sealed record PartnerIdentityTicket(string Subject, string[] Roles);

public static class PartnerIdentityAdapter
{
    // The ticket is produced only after trusted JWT validation.
    public static ClaimsPrincipal FromValidatedTicket(PartnerIdentityTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        var claims = new List<Claim> { new("sub", ticket.Subject) };
        claims.AddRange(ticket.Roles.Select(role => new Claim("roles", role)));

        return new ClaimsPrincipal(
            new ClaimsIdentity(
                claims,
                authenticationType: "Bearer",
                nameType: "sub",
                roleType: "roles"));
    }
}
