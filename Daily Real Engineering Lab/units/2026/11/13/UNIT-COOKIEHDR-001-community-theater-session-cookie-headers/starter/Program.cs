using System;
using System.Collections.Generic;
using System.Linq;

public static class Program
{
    public static IReadOnlyList<string> BuildResponseCookies()
    {
        // Header collection abstraction from the theater's legacy gateway adapter.
        var outgoing = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var session = "theater_session=abc123; Path=/; HttpOnly; Secure; SameSite=Lax";
        var preference = "seat_zone=balcony; Path=/; Secure; SameSite=Lax";
        outgoing["Set-Cookie"] = string.Join(", ", new[] { session, preference });
        return outgoing.TryGetValue("Set-Cookie", out var value) ? new[] { value } : Array.Empty<string>();
    }

    public static int Main(string[] args)
    {
        var headers = BuildResponseCookies();
        Console.WriteLine("Set-Cookie header field count: " + headers.Count);
        foreach (var value in headers) Console.WriteLine("Set-Cookie: " + value);
        bool valid = headers.Count == 2 &&
            headers.Any(x => x.StartsWith("theater_session=", StringComparison.Ordinal)) &&
            headers.Any(x => x.StartsWith("seat_zone=", StringComparison.Ordinal)) &&
            headers.All(x => !x.Contains(", seat_zone=", StringComparison.Ordinal));
        Console.WriteLine("COOKIE_CONTRACT=" + (valid ? "PASS" : "FAIL"));
        return valid ? 0 : 2;
    }
}
