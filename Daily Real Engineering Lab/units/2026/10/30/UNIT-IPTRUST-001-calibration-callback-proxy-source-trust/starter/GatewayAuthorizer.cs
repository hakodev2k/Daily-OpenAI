using System.Net;

namespace Lab;

public static class GatewayAuthorizer
{
    private static readonly HashSet<string> AllowedPartnerIps =
        new(StringComparer.Ordinal) { "203.0.113.10" };

    // remotePeer is the IP address of the connection peer observed by the gateway.
    // forwardedFor is the raw header supplied with the request, if present.
    public static bool IsAuthorized(IPAddress remotePeer, string? forwardedFor)
    {
        IPAddress client = remotePeer;
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            string first = forwardedFor.Split(',')[0].Trim();
            if (IPAddress.TryParse(first, out IPAddress? parsed))
                client = parsed;
        }

        return AllowedPartnerIps.Contains(client.ToString());
    }
}
