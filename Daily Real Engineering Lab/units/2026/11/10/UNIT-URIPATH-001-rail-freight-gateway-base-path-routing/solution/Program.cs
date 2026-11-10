using System;
using System.Collections.Generic;

internal static class Program
{
    private static Uri Compose(Uri gatewayBase, string relativeRoute)
    {
        // Gateway prefix is configured by the operations team.
        if (relativeRoute.StartsWith("/", StringComparison.Ordinal))
            throw new ArgumentException("Route must be relative to the gateway prefix.", nameof(relativeRoute));
        if (!gatewayBase.AbsolutePath.EndsWith("/", StringComparison.Ordinal))
            throw new ArgumentException("Gateway base path must end with '/'.", nameof(gatewayBase));
        return new Uri(gatewayBase, relativeRoute);
    }

    private static int Main(string[] args)
    {
        var cases = new (string Name, Uri Base, string Route, string Expected)[]
        {
            ("production", new Uri("https://freight.example.net/partners/v2/"), "consignments/42", "https://freight.example.net/partners/v2/consignments/42"),
            ("staging", new Uri("https://staging.example.net/api/rail/"), "health", "https://staging.example.net/api/rail/health"),
            ("query", new Uri("https://freight.example.net/partners/v2/"), "consignments?status=open", "https://freight.example.net/partners/v2/consignments?status=open")
        };
        var failures = 0;
        foreach (var item in cases)
        {
            var actual = Compose(item.Base, item.Route).AbsoluteUri;
            bool ok = string.Equals(actual, item.Expected, StringComparison.Ordinal);
            Console.WriteLine($"{item.Name}: expected={item.Expected} actual={actual} {(ok ? "PASS" : "FAIL")}");
            if (!ok) failures++;
        }
        if (args.Length == 1 && args[0] == "--reproduce")
            return failures == cases.Length ? 0 : 2;
        if (args.Length == 1 && args[0] == "--verify")
            return failures == 0 ? 0 : 1;
        Console.WriteLine("Use --reproduce before editing, --verify after editing.");
        return 0;
    }
}
