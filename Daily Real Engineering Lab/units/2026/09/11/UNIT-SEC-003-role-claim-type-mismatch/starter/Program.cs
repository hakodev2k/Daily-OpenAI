using System.Security.Claims;

var claims = new[]
{
    new Claim("sub", "user-123"),
    new Claim("roles", "Admin")
};

var identity = new ClaimsIdentity(claims, authenticationType: "demo");
var principal = new ClaimsPrincipal(identity);

Console.WriteLine($"Authenticated={principal.Identity?.IsAuthenticated}");
Console.WriteLine($"AdminClaimCount={principal.Claims.Count(c => c.Type == "roles" && c.Value == "Admin")}");
Console.WriteLine($"IsInRoleAdmin={principal.IsInRole("Admin")}");
