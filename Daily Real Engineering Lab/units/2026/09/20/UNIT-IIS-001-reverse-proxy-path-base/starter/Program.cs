const string publicMount = "/benefits";
const string upstreamRequestPath = "/account/profile";

var generatedNavigation = BuildNavigation(upstreamRequestPath);

Console.WriteLine($"Public request: {publicMount}{upstreamRequestPath}");
Console.WriteLine($"Upstream sees: {upstreamRequestPath}");
Console.WriteLine($"Generated navigation: {generatedNavigation}");
Console.WriteLine($"Inside public boundary: {generatedNavigation.StartsWith(publicMount + "/", StringComparison.Ordinal)}");

if (args.Contains("--verify", StringComparer.OrdinalIgnoreCase))
    return generatedNavigation == "/benefits/account/settings" ? 0 : 2;

if (args.Contains("--reproduce", StringComparer.OrdinalIgnoreCase))
    return generatedNavigation.StartsWith(publicMount + "/", StringComparison.Ordinal) ? 3 : 0;

return 0;

static string BuildNavigation(string currentPath)
{
    // Investigation note: this helper receives the route as observed by the application.
    // Decide which request identity must be preserved when producing a public URL.
    _ = currentPath;
    return "/account/settings";
}