record RequestEnvelope(string RemoteAddress, string Host, string? ForwardedHost);

static class RecoveryLinkBuilder
{
    public static string Build(RequestEnvelope request, string token)
    {
        // Investigation note: which request values are authoritative at this deployment boundary?
        var publicHost = string.IsNullOrWhiteSpace(request.ForwardedHost)
            ? request.Host
            : request.ForwardedHost;

        return $"https://{publicHost}/account/recover?token={Uri.EscapeDataString(token)}";
    }
}

var mode = args.FirstOrDefault() ?? "demo";
var trusted = new RequestEnvelope("10.0.0.10", "app.internal:8080", "accounts.example.test");
var direct = new RequestEnvelope("203.0.113.44", "app.internal:8080", "external.example.test");

if (mode == "reproduce")
{
    var link = RecoveryLinkBuilder.Build(direct, "abc123");
    Console.WriteLine($"remote={direct.RemoteAddress}");
    Console.WriteLine($"host={direct.Host}");
    Console.WriteLine($"forwardedHost={direct.ForwardedHost}");
    Console.WriteLine($"generated={link}");
    if (link.Contains("external.example.test", StringComparison.OrdinalIgnoreCase))
    {
        Console.Error.WriteLine("REPRODUCED: untrusted request influenced generated public host.");
        Environment.Exit(2);
    }
    Environment.Exit(0);
}

if (mode == "verify")
{
    var trustedLink = RecoveryLinkBuilder.Build(trusted, "ok");
    var directLink = RecoveryLinkBuilder.Build(direct, "ok");
    var trustedOk = trustedLink.StartsWith("https://accounts.example.test/", StringComparison.OrdinalIgnoreCase);
    var directOk = !directLink.Contains("external.example.test", StringComparison.OrdinalIgnoreCase);
    Console.WriteLine($"trusted={trustedLink}");
    Console.WriteLine($"direct={directLink}");
    if (!trustedOk || !directOk)
    {
        Console.Error.WriteLine("VERIFY FAILED: public-origin trust contract is not satisfied.");
        Environment.Exit(3);
    }
    Console.WriteLine("VERIFY PASSED");
    Environment.Exit(0);
}

Console.WriteLine(RecoveryLinkBuilder.Build(trusted, "demo"));