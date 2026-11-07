namespace PermitLab;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var mode = args.Length == 0 ? "observe" : args[0].ToLowerInvariant();
        return mode switch
        {
            "observe" => await ObserveAsync(),
            "reproduce" => await ReproduceAsync(),
            "verify" => await VerifyAsync(),
            _ => Usage()
        };
    }

    private static int Usage()
    {
        Console.Error.WriteLine("Usage: dotnet run -- [observe|reproduce|verify]");
        return 2;
    }

    private static async Task<Observation> ProbeAsync(IPermitStore store)
    {
        var accepted = await new PermitApprovalService(store).ApproveAsync("P-204");
        var reloaded = await store.ReloadAsync("P-204");
        return new Observation(accepted, reloaded?.Status, reloaded?.Version, store.SaveCount);
    }

    private static async Task<int> ObserveAsync()
    {
        var fake = await ProbeAsync(new IdentityMapFakeStore());
        var detached = await ProbeAsync(new DetachedPermitStore());
        Console.WriteLine($"IdentityMapFake: {fake}");
        Console.WriteLine($"DetachedStore:   {detached}");
        return 0;
    }

    private static async Task<int> ReproduceAsync()
    {
        var fake = await ProbeAsync(new IdentityMapFakeStore());
        var detached = await ProbeAsync(new DetachedPermitStore());
        Console.WriteLine($"Unit-test fake: {fake}");
        Console.WriteLine($"Production-like: {detached}");

        var reproduced = fake.Accepted && fake.ReloadedStatus == "Approved"
            && fake.ReloadedVersion == 8 && fake.SaveCount == 0
            && detached.Accepted && detached.ReloadedStatus == "Pending"
            && detached.ReloadedVersion == 7 && detached.SaveCount == 0;

        Console.WriteLine(reproduced
            ? "REPRODUCED: green fake check, but production-like reload remains Pending."
            : "NOT REPRODUCED: original discrepancy is absent.");
        return reproduced ? 0 : 1;
    }

    private static async Task<int> VerifyAsync()
    {
        var failures = new List<string>();
        foreach (IPermitStore store in new IPermitStore[]
                 { new IdentityMapFakeStore(), new DetachedPermitStore() })
        {
            var service = new PermitApprovalService(store);
            var first = await service.ApproveAsync("P-204");
            var snapshot = await store.ReloadAsync("P-204");
            var second = await service.ApproveAsync("P-204");
            var afterSecond = await store.ReloadAsync("P-204");

            Check(first, $"{store.GetType().Name}: first approval should succeed", failures);
            Check(snapshot?.Status == "Approved" && snapshot.Version == 8,
                $"{store.GetType().Name}: persisted state/version incorrect", failures);
            Check(!second, $"{store.GetType().Name}: second approval should be rejected", failures);
            Check(afterSecond?.Status == "Approved" && afterSecond.Version == 8,
                $"{store.GetType().Name}: second call changed persisted state", failures);
            Check(store.SaveCount == 1,
                $"{store.GetType().Name}: expected exactly one explicit persistence write", failures);
        }

        var missingStore = new DetachedPermitStore();
        var missing = await new PermitApprovalService(missingStore).ApproveAsync("UNKNOWN");
        Check(!missing && missingStore.SaveCount == 0,
            "Not-found permit must not be written", failures);

        if (failures.Count > 0)
        {
            foreach (var failure in failures) Console.Error.WriteLine("FAIL: " + failure);
            return 1;
        }

        Console.WriteLine("PASS: persistence, version, duplicate approval, and not-found contract.");
        return 0;
    }

    private static void Check(bool condition, string message, List<string> failures)
    {
        if (!condition) failures.Add(message);
    }

    private sealed record Observation(bool Accepted, string? ReloadedStatus,
        int? ReloadedVersion, int SaveCount);
}
