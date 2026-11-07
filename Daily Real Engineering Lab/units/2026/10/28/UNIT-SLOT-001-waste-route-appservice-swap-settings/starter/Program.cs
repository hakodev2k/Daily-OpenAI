using SlotSwapLab;

try
{
    if (args.Length != 1 || (args[0] != "--reproduce" && args[0] != "--verify"))
        throw new ArgumentException("Use --reproduce or --verify.");

    var reproduce = args[0] == "--reproduce";
    var fixture = reproduce ? "baseline.json" : "slots.json";
    var path = Path.Combine(AppContext.BaseDirectory, "fixtures", fixture);
    var before = SlotSwapEngine.Load(path);
    var after = SlotSwapEngine.Swap(before);

    Print("BEFORE", before);
    Print("AFTER SWAP", after);

    Check(before.Production.Settings["ROUTE_DB_ENDPOINT"].Value == "sql-prod.internal", "Unexpected original production endpoint.");
    Check(before.Staging.Settings["ROUTE_DB_ENDPOINT"].Value == "sql-stage.internal", "Unexpected original staging endpoint.");
    Check(after.Production.Settings["RELEASE_LABEL"].Value == "v2", "Release label should move to production.");
    Check(after.Staging.Settings["RELEASE_LABEL"].Value == "v1", "Old release label should move to staging.");
    Check(after.Production.Settings["FEATURE_NEW_ROUTER"].Value == "true", "New feature flag should follow release.");
    Check(after.Staging.Settings["FEATURE_NEW_ROUTER"].Value == "false", "Old feature flag should follow previous release.");
    Check(after.Production.Settings["SLOT_ENVIRONMENT"].Value == "production", "Production environment label must remain.");
    Check(after.Staging.Settings["SLOT_ENVIRONMENT"].Value == "staging", "Staging environment label must remain.");

    if (reproduce)
    {
        Check(after.Production.Settings["ROUTE_DB_ENDPOINT"].Value == "sql-stage.internal",
            "Baseline symptom not reproduced.");
        Check(after.Staging.Settings["ROUTE_DB_ENDPOINT"].Value == "sql-prod.internal",
            "Baseline staging symptom not reproduced.");
        Console.WriteLine("REPRODUCED: release moved, but production endpoint changed to staging.");
    }
    else
    {
        Check(before.Production.Settings["ROUTE_DB_ENDPOINT"].SlotSetting,
            "Production database setting is not environment-bound.");
        Check(before.Staging.Settings["ROUTE_DB_ENDPOINT"].SlotSetting,
            "Staging database setting is not environment-bound.");
        Check(!before.Production.Settings["RELEASE_LABEL"].SlotSetting &&
              !before.Staging.Settings["RELEASE_LABEL"].SlotSetting,
            "Release label must remain swappable.");
        Check(!before.Production.Settings["FEATURE_NEW_ROUTER"].SlotSetting &&
              !before.Staging.Settings["FEATURE_NEW_ROUTER"].SlotSetting,
            "Release feature flag must remain swappable.");

        Check(after.Production.Settings["ROUTE_DB_ENDPOINT"].Value == "sql-prod.internal",
            "Production must retain production database.");
        Check(after.Staging.Settings["ROUTE_DB_ENDPOINT"].Value == "sql-stage.internal",
            "Staging must retain staging database.");

        var rollback = SlotSwapEngine.Swap(after);
        foreach (var key in before.Production.Settings.Keys)
        {
            Check(rollback.Production.Settings[key].Value == before.Production.Settings[key].Value,
                $"Production rollback mismatch: {key}");
            Check(rollback.Staging.Settings[key].Value == before.Staging.Settings[key].Value,
                $"Staging rollback mismatch: {key}");
        }

        Console.WriteLine("VERIFIED: environment isolation, release movement, and rollback invariants.");
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"CHECK FAILED: {ex}");
    Environment.ExitCode = 1;
}

static void Print(string stage, DeploymentConfig snapshot)
{
    Console.WriteLine(stage);
    foreach (var (label, slot) in new[]
    {
        ("production", snapshot.Production),
        ("staging", snapshot.Staging)
    })
    {
        foreach (var entry in slot.Settings.OrderBy(x => x.Key, StringComparer.Ordinal))
            Console.WriteLine($"  {label}.{entry.Key}={entry.Value.Value} [slotSetting={entry.Value.SlotSetting}]");
    }
}

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
