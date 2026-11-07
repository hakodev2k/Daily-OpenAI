using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace RoleClaimLab;

public static class ReviewAuthorization
{
    private static readonly AuthorizationPolicy ReviewerPolicy =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("Reviewer")
            .Build();

    public static async Task<bool> IsReviewerAsync(ClaimsPrincipal principal)
    {
        using var provider = new ServiceCollection()
            .AddLogging()
            .AddAuthorization()
            .BuildServiceProvider();

        var authorization = provider.GetRequiredService<IAuthorizationService>();
        var result = await authorization.AuthorizeAsync(principal, null, ReviewerPolicy);
        return result.Succeeded;
    }
}
