using System.Net;
using Lab;

var mode = args.Length == 0 ? "inspect" : args[0];
if (mode is not ("inspect" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run -- [inspect|reproduce|verify]");
    return 64;
}

// Contract: the only trusted one-hop proxy is 10.0.0.10.
// Only partner 203.0.113.10 is permitted through that proxy.
Fixture[] fixtures =
[
    new("spoofed-direct", "198.51.100.77", "203.0.113.10", false),
    new("valid-proxy-partner", "10.0.0.10", "203.0.113.10", true),
    new("proxy-unknown-partner", "10.0.0.10", "198.51.100.77", false),
    new("proxy-no-header", "10.0.0.10", null, false),
    new("proxy-malformed-header", "10.0.0.10", "not-an-ip", false),
    new("proxy-multiple-hops", "10.0.0.10", "203.0.113.10, 198.51.100.77", false),
    new("direct-allowed-ip-no-proxy", "203.0.113.10", null, false),
    new("valid-proxy-whitespace", "10.0.0.10", " 203.0.113.10 ", true),
    new("proxy-unapproved-ipv6", "10.0.0.10", "2001:db8::42", false)
];

int mismatches = 0;
bool spoofReproduced = false;
foreach (var fixture in fixtures)
{
    bool actual = GatewayAuthorizer.IsAuthorized(
        IPAddress.Parse(fixture.RemotePeer), fixture.ForwardedFor);
    bool matches = actual == fixture.Expected;
    if (!matches) mismatches++;
    if (fixture.Name == "spoofed-direct" && actual && !fixture.Expected)
        spoofReproduced = true;

    Console.WriteLine(
        $"{fixture.Name,-27} remote={fixture.RemotePeer,-15} xff={fixture.ForwardedFor ?? "<missing>",-34} " +
        $"expected={(fixture.Expected ? "ALLOW" : "DENY"),-5} actual={(actual ? "ALLOW" : "DENY"),-5} " +
        $"{(matches ? "PASS" : "MISMATCH")}");
}

Console.WriteLine($"Cases={fixtures.Length}; mismatches={mismatches}; spoofReproduced={spoofReproduced}");
return mode switch
{
    "reproduce" => spoofReproduced ? 0 : 1,
    "verify" => mismatches == 0 ? 0 : 1,
    _ => 0
};

internal sealed record Fixture(
    string Name, string RemotePeer, string? ForwardedFor, bool Expected);
