using System.Text.Json;

public sealed record Decision(bool Accepted, int? PersistedAmount, string Reason);

public static class GrantWorkflow
{
    private const int MaxAutomaticGrant = 5_000;

    public static Decision Handle(string rawJson)
    {
        try
        {
            using var document = JsonDocument.Parse(rawJson);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return Reject("object-required");

            var seen = new HashSet<string>(StringComparer.Ordinal);
            string? requestId = null;
            int? amount = null;

            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (!seen.Add(property.Name))
                    return Reject("duplicate-property");

                switch (property.Name)
                {
                    case "requestId":
                        if (property.Value.ValueKind != JsonValueKind.String)
                            return Reject("invalid-request-id");
                        requestId = property.Value.GetString();
                        break;
                    case "grantAmount":
                        if (property.Value.ValueKind != JsonValueKind.Number ||
                            !property.Value.TryGetInt32(out var parsedAmount))
                            return Reject("invalid-amount");
                        amount = parsedAmount;
                        break;
                    default:
                        return Reject("unknown-property");
                }
            }

            if (string.IsNullOrWhiteSpace(requestId) || amount is null)
                return Reject("required-fields-missing");
            if (amount <= 0 || amount > MaxAutomaticGrant)
                return Reject("manual-review-required");

            // A single validated value is used for both approval and posting.
            return new Decision(true, amount.Value, "posted");
        }
        catch (JsonException)
        {
            return Reject("invalid-json");
        }
    }

    private static Decision Reject(string reason) => new(false, null, reason);
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
