var forwardedProto = Environment.GetEnvironmentVariable("FORWARDED_PROTO") ?? "";
var connectionScheme = Environment.GetEnvironmentVariable("CONNECTION_SCHEME") ?? "http";

// Starter intentionally makes the redirect decision before applying the trusted proxy scheme.
var schemeSeenByRedirect = connectionScheme;
var shouldRedirect = !string.Equals(schemeSeenByRedirect, "https", StringComparison.OrdinalIgnoreCase);

Console.WriteLine($"ConnectionScheme={connectionScheme}");
Console.WriteLine($"X-Forwarded-Proto={forwardedProto}");
Console.WriteLine($"SchemeSeenByRedirect={schemeSeenByRedirect}");
Console.WriteLine(shouldRedirect ? "REDIRECT_HTTPS" : "SERVE_REQUEST");
