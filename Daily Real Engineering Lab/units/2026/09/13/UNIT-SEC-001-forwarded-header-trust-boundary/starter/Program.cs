using System.Net;

var trustedProxy = IPAddress.Parse("10.0.0.10");
var allowedOperator = IPAddress.Parse("10.20.30.40");

var scenarios = new[]
{
    new RequestCase("direct-normal", IPAddress.Parse("203.0.113.50"), null, false),
    new RequestCase("direct-with-forwarded-header", IPAddress.Parse("203.0.113.50"), "10.20.30.40", false),
    new RequestCase("trusted-proxy-operator", trustedProxy, "10.20.30.40", true),
    new RequestCase("trusted-proxy-other-client", trustedProxy, "198.51.100.25", false)
};

var failed = false;
foreach (var request in scenarios)
{
    var effectiveClient = ResolveEffectiveClient(request.NetworkPeer, request.ForwardedFor);
    var authorized = effectiveClient.Equals(allowedOperator);
    var pass = authorized == request.ExpectedAuthorized;

    Console.WriteLine($"{request.Name}: peer={request.NetworkPeer}, forwarded={request.ForwardedFor ?? "<none>"}, effective={effectiveClient}, authorized={authorized}, expected={request.ExpectedAuthorized}, {(pass ? "PASS" : "FAIL")}");
    failed |= !pass;
}

return failed ? 1 : 0;

IPAddress ResolveEffectiveClient(IPAddress networkPeer, string? forwardedFor)
{
    // Investigation note:
    // Which network participant is allowed to assert the original client identity?
    if (IPAddress.TryParse(forwardedFor, out var forwardedClient))
    {
        return forwardedClient;
    }

    return networkPeer;
}

internal sealed record RequestCase(
    string Name,
    IPAddress NetworkPeer,
    string? ForwardedFor,
    bool ExpectedAuthorized);
