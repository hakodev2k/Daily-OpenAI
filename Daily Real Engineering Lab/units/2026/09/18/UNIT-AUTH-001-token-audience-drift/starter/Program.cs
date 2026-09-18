record TokenClaims(string Issuer, string Audience, DateTimeOffset ExpiresAt);
record AuthSettings(string Issuer, string Audience);

static bool Validate(TokenClaims token, AuthSettings settings, DateTimeOffset now)
{
    return token.ExpiresAt > now
        && token.Issuer == settings.Issuer
        && token.Audience == settings.Audience;
}

var now = DateTimeOffset.Parse("2026-09-18T21:00:00+07:00");
var token = new TokenClaims("https://identity.example.test", "reporting-api-prod", now.AddHours(1));
var staging = new AuthSettings("https://identity.example.test", "reporting-api-staging");
var production = new AuthSettings("https://identity.example.test", "reporting-api-staging");
var otherServiceToken = token with { Audience = "billing-api-prod" };
var mode = args.FirstOrDefault() ?? "run";

if (mode == "reproduce")
{
    var stagingToken = token with { Audience = "reporting-api-staging" };
    Console.WriteLine($"staging={Validate(stagingToken, staging, now)}");
    Console.WriteLine($"production={Validate(token, production, now)}");
    if (Validate(stagingToken, staging, now) && !Validate(token, production, now)) Environment.Exit(2);
    Environment.Exit(0);
}

if (mode == "verify")
{
    var acceptsExpected = Validate(token, production, now);
    var rejectsOther = !Validate(otherServiceToken, production, now);
    Console.WriteLine($"expected-token={acceptsExpected}");
    Console.WriteLine($"other-service-token-rejected={rejectsOther}");
    if (!acceptsExpected || !rejectsOther) Environment.Exit(3);
    Environment.Exit(0);
}

Console.WriteLine($"production-validation={Validate(token, production, now)}");