using System.Text.Json;

public sealed record Decision(bool Accepted, int? PersistedAmount, string Reason);

public static class GrantWorkflow
{
    private const int MaxAutomaticGrant = 5_000;

    public static Decision Handle(string rawJson)
    {
        // The intake gate and the accounting writer are maintained by separate teams.
        using var intake = JsonDocument.Parse(rawJson);
        var checkedAmount = intake.RootElement.EnumerateObject()
            .First(p => p.NameEquals("grantAmount")).Value.GetInt32();

        if (checkedAmount <= 0 || checkedAmount > MaxAutomaticGrant)
            return new Decision(false, null, "manual-review-required");

        using var accounting = JsonDocument.Parse(rawJson);
        var postedAmount = accounting.RootElement.GetProperty("grantAmount").GetInt32();
        return new Decision(true, postedAmount, "posted");
    }
}

public static class Program
{
    public static int Main(string[] args)
    {
        const string sample = "{\"requestId\":\"LIB-204\",\"grantAmount\":4200,\"grantAmount\":42000}";
        var result = GrantWorkflow.Handle(sample);
        Console.WriteLine($"accepted={result.Accepted}; persisted={result.PersistedAmount}; reason={result.Reason}");
        if (args.Length > 0 && args[0] == "--reproduce")
            return result.Accepted && result.PersistedAmount == 42_000 ? 0 : 1;
        return 0;
    }
}
