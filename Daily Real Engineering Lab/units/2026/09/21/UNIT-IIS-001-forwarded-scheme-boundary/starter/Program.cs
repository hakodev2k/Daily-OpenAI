var forwardedProto = Environment.GetEnvironmentVariable("SIM_X_FORWARDED_PROTO") ?? "";
var connectionScheme = Environment.GetEnvironmentVariable("SIM_CONNECTION_SCHEME") ?? "http";

// Mô phỏng pipeline hiện tại: redirect policy chạy trước khi application
// thiết lập trusted proxy boundary và reconstruct original request scheme.
var effectiveScheme = connectionScheme;
var redirect = !string.Equals(effectiveScheme, "https", StringComparison.OrdinalIgnoreCase);

Console.WriteLine($"Connection scheme: {connectionScheme}");
Console.WriteLine($"Forwarded proto evidence: {forwardedProto}");
Console.WriteLine($"Effective scheme: {effectiveScheme}");
Console.WriteLine(redirect ? "ACTION=REDIRECT_HTTPS" : "ACTION=SERVE_REQUEST");