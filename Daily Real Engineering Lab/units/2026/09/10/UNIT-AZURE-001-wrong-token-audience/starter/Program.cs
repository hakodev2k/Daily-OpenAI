const string configuredScope = "https://management.azure.com/.default";
const string expectedAudience = "https://storage.azure.com/";

var mode = args.FirstOrDefault() ?? "run";
var scope = mode == "reproduce" ? "https://management.azure.com/.default" : configuredScope;

var credential = new FakeManagedIdentityCredential();
var token = await credential.GetTokenAsync(scope);
var resource = new FakeBlobResource(expectedAudience);
var status = resource.Send(token);

Console.WriteLine($"TOKEN_ACQUIRED={token is not null}");
Console.WriteLine($"TOKEN_AUDIENCE={token.Audience}");
Console.WriteLine($"DOWNSTREAM_STATUS={status}");

if (mode == "verify")
{
    var pass = token.Audience == expectedAudience && status == 200;
    Console.WriteLine(pass ? "LAB_VERIFY_PASS" : "LAB_VERIFY_FAIL");
    Environment.ExitCode = pass ? 0 : 1;
}
else if (mode == "reproduce")
{
    var reproduced = token.Audience != expectedAudience && status == 401;
    Console.WriteLine(reproduced ? "LAB_REPRODUCE_PASS" : "LAB_REPRODUCE_FAIL");
    Environment.ExitCode = reproduced ? 0 : 1;
}

sealed record AccessToken(string Audience);

sealed class FakeManagedIdentityCredential
{
    public Task<AccessToken> GetTokenAsync(string scope)
    {
        var suffix = ".default";
        if (!scope.EndsWith(suffix, StringComparison.Ordinal))
            throw new InvalidOperationException("Scope must end with .default in this simulation.");

        var audience = scope[..^suffix.Length];
        return Task.FromResult(new AccessToken(audience));
    }
}

sealed class FakeBlobResource(string expectedAudience)
{
    public int Send(AccessToken token)
        => StringComparer.Ordinal.Equals(token.Audience, expectedAudience) ? 200 : 401;
}
