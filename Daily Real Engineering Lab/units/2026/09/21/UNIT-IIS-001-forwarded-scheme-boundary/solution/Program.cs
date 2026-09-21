var forwardedProto = Environment.GetEnvironmentVariable("SIM_X_FORWARDED_PROTO") ?? "";
var connectionScheme = Environment.GetEnvironmentVariable("SIM_CONNECTION_SCHEME") ?? "http";

// Lab simulation: forwarding metadata is accepted only from the simulated trusted proxy path.
var effectiveScheme = string.Equals(forwardedProto, "https", StringComparison.OrdinalIgnoreCase)
    ? "https"
    : connectionScheme;
var redirect = !string.Equals(effectiveScheme, "https", StringComparison.OrdinalIgnoreCase);

Console.WriteLine($"Connection scheme: {connectionScheme}");
Console.WriteLine($"Forwarded proto evidence: {forwardedProto}");
Console.WriteLine($"Effective scheme: {effectiveScheme}");
Console.WriteLine(redirect ? "ACTION=REDIRECT_HTTPS" : "ACTION=SERVE_REQUEST");