using Microsoft.AspNetCore.Http;

var mode = args.FirstOrDefault() ?? "run";
var request = new SimulatedRequest(
    Scheme: "https",
    Host: "portal.company.test",
    PathBase: "",
    Path: "/profile",
    ForwardedPrefix: "/staff");

var generated = LinkBuilder.BuildProfileUrl(request);
Console.WriteLine($"proxy-prefix={request.ForwardedPrefix}");
Console.WriteLine($"app-pathbase={request.PathBase}");
Console.WriteLine($"generated={generated}");

var expected = "https://portal.company.test/staff/profile";
if (mode is "reproduce")
{
    if (generated == expected)
    {
        Console.Error.WriteLine("Starter symptom was not reproduced.");
        return 2;
    }

    Console.WriteLine($"expected-public-url={expected}");
    Console.WriteLine("SYMPTOM_REPRODUCED");
    return 0;
}

if (mode is "verify")
{
    if (generated != expected)
    {
        Console.Error.WriteLine($"VERIFY_FAIL expected={expected} actual={generated}");
        return 3;
    }

    Console.WriteLine("VERIFY_PASS");
    return 0;
}

return 0;

internal sealed record SimulatedRequest(
    string Scheme,
    string Host,
    string PathBase,
    string Path,
    string? ForwardedPrefix);

internal static class LinkBuilder
{
    public static string BuildProfileUrl(SimulatedRequest request)
    {
        // Investigation note: the proxy may expose a different public request shape
        // from the path received by the application.
        var effectivePathBase = request.PathBase;
        return $"{request.Scheme}://{request.Host}{effectivePathBase}{request.Path}";
    }
}