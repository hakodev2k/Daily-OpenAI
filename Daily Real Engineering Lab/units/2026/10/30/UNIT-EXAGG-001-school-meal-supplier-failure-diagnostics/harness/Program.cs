using MealReportLab;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var mode = args.Length > 0 ? args[0].ToLowerInvariant() : "run";
        if (mode is not ("run" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run --project harness/LabHarness.csproj -- [run|reproduce|verify]");
            return 2;
        }

        bool healthy = await HealthyCaseAsync();
        bool faults = await MultipleFaultsCaseAsync();
        bool cancelled = await CancellationCaseAsync();
        Console.WriteLine($"SUMMARY healthy={healthy} all_faults_visible={faults} cancellation_propagated={cancelled}");

        if (mode == "run") return 0;
        if (mode == "reproduce")
        {
            bool symptomObserved = !faults || !cancelled;
            Console.WriteLine(symptomObserved ? "REPRODUCED: starter violates diagnostic contract" : "NOT_REPRODUCED: original symptoms absent");
            return healthy && symptomObserved ? 0 : 1;
        }

        bool verified = healthy && faults && cancelled;
        Console.WriteLine(verified ? "VERIFIED: all contracts satisfied" : "FAILED: learner code still violates at least one contract");
        return verified ? 0 : 1;
    }

    private static async Task<bool> HealthyCaseAsync()
    {
        var outcome = await SupplierReport.BuildAsync(new Func<Task<SupplierResult>>[]
        {
            () => Task.FromResult(new SupplierResult("orchard", 3)),
            () => Task.FromResult(new SupplierResult("dairy", 4))
        });
        bool pass = outcome.Published && outcome.TotalItems == 7 && outcome.Diagnostics.Count == 0;
        Console.WriteLine($"healthy: published={outcome.Published} total={outcome.TotalItems} diagnostics={outcome.Diagnostics.Count} pass={pass}");
        return pass;
    }

    private static async Task<bool> MultipleFaultsCaseAsync()
    {
        var outcome = await SupplierReport.BuildAsync(new Func<Task<SupplierResult>>[]
        {
            () => Task.FromException<SupplierResult>(new SupplierFeedException("orchard", "HTTP_503")),
            () => Task.FromResult(new SupplierResult("bakery", 5)),
            () => Task.FromException<SupplierResult>(new SupplierFeedException("dairy", "INVALID_SCHEMA"))
        });
        string[] expected = { "orchard:HTTP_503", "dairy:INVALID_SCHEMA" };
        bool pass = !outcome.Published
            && outcome.TotalItems == 0
            && outcome.Diagnostics.Count == 2
            && expected.All(item => outcome.Diagnostics.Contains(item, StringComparer.Ordinal));
        Console.WriteLine($"multiple_faults: published={outcome.Published} diagnostics=[{string.Join(", ", outcome.Diagnostics)}] pass={pass}");
        return pass;
    }

    private static async Task<bool> CancellationCaseAsync()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        try
        {
            var outcome = await SupplierReport.BuildAsync(new Func<Task<SupplierResult>>[]
            {
                () => Task.FromCanceled<SupplierResult>(source.Token)
            });
            Console.WriteLine($"cancellation: returned published={outcome.Published} diagnostics={outcome.Diagnostics.Count} pass=False");
            return false;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("cancellation: propagated OperationCanceledException pass=True");
            return true;
        }
    }
}
