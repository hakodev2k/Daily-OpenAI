const string configuredScope = "https://storage.azure.com/.default";
const string expectedAudience = "https://storage.azure.com/";

var credential = new FakeManagedIdentityCredential();
var token = await credential.GetTokenAsync(configuredScope);
var resource = new FakeBlobResource(expectedAudience);
var status = resource.Send(token);

Console.WriteLine($"TOKEN_ACQUIRED={token is not null}");
Console.WriteLine($"TOKEN_AUDIENCE={token.Audience}");
Console.WriteLine($"DOWNSTREAM_STATUS={status}");
Console.WriteLine(status == 200 ? "REFERENCE_PASS" : "REFERENCE_FAIL");

sealed record AccessToken(string Audience);

sealed class FakeManagedIdentityCredential
{
    public Task<AccessToken> GetTokenAsync(string scope)
    {
        const string suffix = ".default";
        if (!scope.EndsWith(suffix, StringComparison.Ordinal))
            throw new InvalidOperationException("Scope must end with .default in this simulation.");

        return Task.FromResult(new AccessToken(scope[..^suffix.Length]));
    }
}

sealed class FakeBlobResource(string expectedAudience)
{
    public int Send(AccessToken token)
        => StringComparer.Ordinal.Equals(token.Audience, expectedAudience) ? 200 : 401;
}
