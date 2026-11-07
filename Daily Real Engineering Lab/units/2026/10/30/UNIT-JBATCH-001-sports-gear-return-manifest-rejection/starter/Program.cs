using System.Text;
using System.Text.Json;
using IntakeLab;

return args.Length > 0 && args[0] == "--verify"
    ? await ContractRunner.VerifyAsync()
    : await ContractRunner.ReproduceAsync(args.Length > 0 && args[0] == "--reproduce");

internal static class ContractRunner
{
    private const string InvalidJson = """
        [
          {"Id":"GEAR-101","Qty":2},
          {"Id":"GEAR-102","Qty":1},
          {"Id":"GEAR-103","Qty":"not-a-number"}
        ]
        """;

    private const string InvalidBusiness = """
        [
          {"Id":"GEAR-201","Qty":2},
          {"Id":"GEAR-202","Qty":-1}
        ]
        """;

    private const string ValidJson = """
        [
          {"Id":"GEAR-301","Qty":3},
          {"Id":"GEAR-302","Qty":4},
          {"Id":"GEAR-303","Qty":5}
        ]
        """;

    private static MemoryStream Input(string json) =>
        new(Encoding.UTF8.GetBytes(json));

    public static async Task<int> ReproduceAsync(bool strict)
    {
        var store = new ManifestStore();
        var rejected = false;
        try
        {
            using var input = Input(InvalidJson);
            await new BatchImporter().ImportAsync(input, store);
        }
        catch (JsonException)
        {
            rejected = true;
        }

        Console.WriteLine($"Rejected={rejected}; CommittedCount={store.Count}");
        Console.WriteLine("ObservedIds=" + string.Join(",", store.Snapshot().Keys.Order()));
        var reproduced = rejected && store.Count == 2;
        Console.WriteLine("Reproduction=" + (reproduced ? "PASS" : "NOT_REPRODUCED"));
        return !strict || reproduced ? 0 : 1;
    }

    public static async Task<int> VerifyAsync()
    {
        var importer = new BatchImporter();

        var syntaxStore = new ManifestStore();
        var syntaxRejected = false;
        try
        {
            using var input = Input(InvalidJson);
            await importer.ImportAsync(input, syntaxStore);
        }
        catch (JsonException)
        {
            syntaxRejected = true;
        }

        var syntaxOk = syntaxRejected && syntaxStore.Count == 0;
        Console.WriteLine($"MalformedJsonRejected={syntaxRejected}; CommittedCount={syntaxStore.Count}");

        var businessStore = new ManifestStore();
        var businessRejected = false;
        try
        {
            using var input = Input(InvalidBusiness);
            await importer.ImportAsync(input, businessStore);
        }
        catch (InvalidDataException)
        {
            businessRejected = true;
        }

        var businessOk = businessRejected && businessStore.Count == 0;
        Console.WriteLine($"InvalidBusinessRejected={businessRejected}; CommittedCount={businessStore.Count}");

        var validStore = new ManifestStore();
        using (var input = Input(ValidJson))
            await importer.ImportAsync(input, validStore);
        var snapshot = validStore.Snapshot();
        var validOk = snapshot.Count == 3 &&
            snapshot.TryGetValue("GEAR-301", out var q1) && q1 == 3 &&
            snapshot.TryGetValue("GEAR-302", out var q2) && q2 == 4 &&
            snapshot.TryGetValue("GEAR-303", out var q3) && q3 == 5;
        Console.WriteLine($"ValidBatchCount={snapshot.Count}; ValidValues={validOk}");

        var pass = syntaxOk && businessOk && validOk;
        Console.WriteLine(pass ? "CONTRACT_PASS" : "CONTRACT_FAIL");
        return pass ? 0 : 1;
    }
}
