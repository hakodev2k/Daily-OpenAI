using System.Text.Json;

public static class GrantContract
{
    public static (bool audit, bool payout) Process(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        // Legacy audit adapter and payout worker were independently maintained.
        bool audit = root.EnumerateObject().First(p => p.Name == "approved").Value.GetBoolean();
        bool payout = root.GetProperty("approved").GetBoolean();
        return (audit, payout);
    }

    public static bool TryProcess(string json, out (bool audit, bool payout) result)
    {
        result = default;
        try
        {
            result = Process(json);
            return true;
        }
        catch (JsonException) { return false; }
        catch (InvalidOperationException) { return false; }
        catch (KeyNotFoundException) { return false; }
    }
}

public static class Program
{
    public static int Main(string[] args)
    {
        const string incident = "{\"grantId\":\"RELIEF-104\",\"approved\":false,\"approved\":true}";
        const string valid = "{\"grantId\":\"RELIEF-105\",\"approved\":true}";
        if (args.Contains("--verify"))
        {
            bool rejected = !GrantContract.TryProcess(incident, out _);
            bool accepted = GrantContract.TryProcess(valid, out var safe) && safe.audit && safe.payout;
            Console.WriteLine($"Ambiguous payload rejected: {rejected}");
            Console.WriteLine($"Valid payload accepted: {accepted}");
            return rejected && accepted ? 0 : 1;
        }
        var decision = GrantContract.Process(incident);
        Console.WriteLine($"AUDIT approved={decision.audit}");
        Console.WriteLine($"PAYOUT approved={decision.payout}");
        return decision.audit == decision.payout ? 0 : 2;
    }
}
