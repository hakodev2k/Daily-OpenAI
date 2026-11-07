const string CookieName = "merchant_session";
const string CookiePath = "/admin";

var routes = new[] { "/admin/orders", "/reports/daily" };
Console.WriteLine($"LOGIN set-cookie: {CookieName}=abc123; Path={CookiePath}; HttpOnly");

foreach (var route in routes)
{
    var sendsCookie = CookieMatchesPath(CookiePath, route);
    Console.WriteLine($"GET {route} | cookie-sent={sendsCookie} | status={(sendsCookie ? 200 : 401)}");
}

static bool CookieMatchesPath(string cookiePath, string requestPath)
{
    if (!requestPath.StartsWith(cookiePath, StringComparison.Ordinal)) return false;
    if (requestPath.Length == cookiePath.Length) return true;
    if (cookiePath.EndsWith('/')) return true;
    return requestPath[cookiePath.Length] == '/';
}
