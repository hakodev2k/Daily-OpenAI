using System.Net;
using System.Net.Sockets;

namespace Lab;

// Reference for the stated one-hop laboratory topology only.
public static class GatewayAuthorizer
{
    private static readonly IPAddress TrustedProxy = IPAddress.Parse("10.0.0.10");
    private static readonly IPAddress AllowedPartner = IPAddress.Parse("203.0.113.10");

    public static bool IsAuthorized(IPAddress remotePeer, string? forwardedFor)
    {
        if (!remotePeer.Equals(TrustedProxy))
            return false;

        // Exactly one managed proxy is permitted. No unverified multi-hop chain.
        if (string.IsNullOrWhiteSpace(forwardedFor) || forwardedFor.Contains(','))
            return false;

        if (!IPAddress.TryParse(forwardedFor.Trim(), out IPAddress? client))
            return false;

        // This exercise's partner allowlist is explicitly IPv4-only.
        return client.AddressFamily == AddressFamily.InterNetwork
            && client.Equals(AllowedPartner);
    }
}
