const string invoiceApiAudience = "api://invoice-service";
const string requestedScope = "https://management.azure.com/.default";

var identity = new FakeIdentityProvider();
var api = new FakeInvoiceApi(invoiceApiAudience);

var token = identity.GetToken(requestedScope);
Console.WriteLine($"identity: token-issued=true expires={token.ExpiresAt:O}");
Console.WriteLine($"client: bearer-present=true");

var response = api.PostInvoice(token);
Console.WriteLine($"api: observed-audience={token.Audience}");
Console.WriteLine($"http-status={response}");

if (args.Contains("reproduce"))
{
    if (response == 401)
    {
        Console.WriteLine("REPRODUCED");
        return 0;
    }

    Console.WriteLine("REPRODUCE_FAIL");
    return 2;
}

if (args.Contains("verify"))
{
    if (response == 200)
    {
        Console.WriteLine("VERIFY_PASS");
        return 0;
    }

    Console.WriteLine("VERIFY_FAIL");
    return 3;
}

return response == 200 ? 0 : 1;

sealed record AccessToken(string Audience, DateTimeOffset ExpiresAt);

sealed class FakeIdentityProvider
{
    public AccessToken GetToken(string scope)
    {
        const string suffix = "/.default";
        var audience = scope.EndsWith(suffix, StringComparison.Ordinal)
            ? scope[..^suffix.Length]
            : scope;
        return new AccessToken(audience, DateTimeOffset.UtcNow.AddMinutes(30));
    }
}

sealed class FakeInvoiceApi(string expectedAudience)
{
    public int PostInvoice(AccessToken token)
    {
        if (token.ExpiresAt <= DateTimeOffset.UtcNow) return 401;
        return string.Equals(token.Audience, expectedAudience, StringComparison.Ordinal) ? 200 : 401;
    }
}
