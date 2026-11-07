using System.Text.Json;

namespace ApprovalAuditLab;

internal static class Program
{
    private static readonly JsonSerializerOptions Options =
        new(JsonSerializerDefaults.Web) { WriteIndented = true };

    private static int Main(string[] args)
    {
        var json = SerializeDemo();
        return (args.Length == 0 ? "run" : args[0].ToLowerInvariant()) switch
        {
            "run" => Run(json),
            "reproduce" => Reproduce(json),
            "verify" => Verify(json),
            _ => 2
        };
    }

    private static IReadOnlyList<ApprovalEvent> DemoEvents() =>
    [
        new ApprovalGranted
        {
            EventId = "APP-1001",
            OccurredAt = new DateTimeOffset(2026, 10, 20, 9, 0, 0, TimeSpan.FromHours(7)),
            ApprovedBy = "reviewer-17",
            PolicyVersion = "p-4"
        },
        new ApprovalRejected
        {
            EventId = "APP-1002",
            OccurredAt = new DateTimeOffset(2026, 10, 20, 9, 5, 0, TimeSpan.FromHours(7)),
            ReasonCode = "MISSING_EVIDENCE"
        }
    ];

    private static string SerializeDemo() =>
        JsonSerializer.Serialize<IReadOnlyList<ApprovalEvent>>(DemoEvents(), Options);

    private static int Run(string json)
    {
        Console.WriteLine("Persisted audit payload:");
        Console.WriteLine(json);
        Console.WriteLine("Input had two concrete events. Compare their fields with the payload.");
        return 0;
    }

    private static int Reproduce(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var items = doc.RootElement.EnumerateArray().ToArray();
        var missing = items.Length == 2
            && !items[0].TryGetProperty("approvedBy", out _)
            && !items[1].TryGetProperty("reasonCode", out _)
            && !items[0].TryGetProperty("$eventType", out _);
        Console.WriteLine(json);
        Console.WriteLine(missing ? "REPRODUCED: audit fields missing." : "NOT REPRODUCED.");
        return missing ? 0 : 1;
    }

    private static int Verify(string json)
    {
        var failures = 0;
        using var doc = JsonDocument.Parse(json);
        var items = doc.RootElement.EnumerateArray().ToArray();
        Check("two events persisted", items.Length == 2, ref failures);
        if (items.Length != 2) return 1;
        Check("granted discriminator", Has(items[0], "$eventType", "approval-granted"), ref failures);
        Check("rejected discriminator", Has(items[1], "$eventType", "approval-rejected"), ref failures);
        Check("granted details", Has(items[0], "approvedBy", "reviewer-17") && Has(items[0], "policyVersion", "p-4"), ref failures);
        Check("rejected details", Has(items[1], "reasonCode", "MISSING_EVIDENCE"), ref failures);
        Check("event identities", Has(items[0], "eventId", "APP-1001") && Has(items[1], "eventId", "APP-1002"), ref failures);

        try
        {
            var replay = JsonSerializer.Deserialize<List<ApprovalEvent>>(json, Options);
            Check("round-trip concrete types and values",
                replay is { Count: 2 }
                && replay[0] is ApprovalGranted { ApprovedBy: "reviewer-17", PolicyVersion: "p-4" }
                && replay[1] is ApprovalRejected { ReasonCode: "MISSING_EVIDENCE" },
                ref failures);
        }
        catch (Exception e) when (e is JsonException or NotSupportedException)
        {
            Console.WriteLine($"Replay error: {e.GetType().Name}");
            Check("round-trip concrete types and values", false, ref failures);
        }

        if (json.Contains("approval-granted", StringComparison.Ordinal))
        {
            var unknown = json.Replace("approval-granted", "future-event-kind", StringComparison.Ordinal);
            try
            {
                _ = JsonSerializer.Deserialize<List<ApprovalEvent>>(unknown, Options);
                Check("unknown event type rejected", false, ref failures);
            }
            catch (JsonException)
            {
                Check("unknown event type rejected", true, ref failures);
            }
        }
        else Check("unknown event type rejected", false, ref failures);

        Console.WriteLine(failures == 0 ? "VERIFIED: all checks passed." : $"NOT VERIFIED: {failures} checks failed.");
        return failures == 0 ? 0 : 1;
    }

    private static bool Has(JsonElement item, string name, string value) =>
        item.TryGetProperty(name, out var property)
        && property.ValueKind == JsonValueKind.String
        && property.GetString() == value;

    private static void Check(string name, bool ok, ref int failures)
    {
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name}");
        if (!ok) failures++;
    }
}
