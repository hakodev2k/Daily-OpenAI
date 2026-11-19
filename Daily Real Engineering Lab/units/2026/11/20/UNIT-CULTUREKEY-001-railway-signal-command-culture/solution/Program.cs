using System.Globalization;

namespace SignalDispatch;

public static class CommandRouter
{
    private static readonly Dictionary<string, string> Routes = new()
    {
        ["SIGNAL_OPEN"] = "relay:open",
        ["SIGNAL_CLOSE"] = "relay:close"
    };

    public static string Route(string command)
    {
        var normalized = command.ToUpperInvariant();
        return Routes.TryGetValue(normalized, out var route) ? route : "REJECTED";
    }
}

public static class Program
{
    public static int Main(string[] args)
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            var cases = new[] { "en-US", "tr-TR" };
            var outputs = new List<string>();
            foreach (var culture in cases)
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                var actual = CommandRouter.Route("signal_open");
                outputs.Add(actual);
                Console.WriteLine($"culture={culture}; command=signal_open; route={actual}");
            }
            if (args.Contains("--verify"))
            {
                var expected = "relay:open";
                if (outputs.Any(x => x != expected))
                {
                    Console.Error.WriteLine("FAIL: command mapping differs across supported host cultures");
                    return 1;
                }
                Console.WriteLine("PASS: command mapping stable across cultures");
            }
            return 0;
        }
        finally { CultureInfo.CurrentCulture = original; }
    }
}
