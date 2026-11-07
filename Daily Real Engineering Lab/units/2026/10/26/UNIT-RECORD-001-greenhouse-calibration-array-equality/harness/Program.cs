using GreenhouseLab;

return args.FirstOrDefault()?.ToLowerInvariant() switch
{
    "reproduce" => Reproduce(),
    "verify" => Verify(),
    "run" => Run(),
    _ => Usage()
};

static int Usage()
{
    Console.Error.WriteLine("Usage: dotnet run --project harness/GreenhouseLab.Harness.csproj -- [run|reproduce|verify]");
    return 2;
}

static int Run()
{
    var sync = new CalibrationSync();
    sync.Observe(new("zone-A", 7, new[] { 18, 35, 62 }));
    sync.Observe(new("zone-A", 7, new[] { 18, 35, 62 }));
    Console.WriteLine($"Controller writes: {sync.AppliedZones.Count}");
    return 0;
}

static int Reproduce()
{
    var sync = new CalibrationSync();
    for (int i = 0; i < 3; i++)
    {
        sync.Observe(new("zone-A", 7, new[] { 18, 35, 62 }));
    }

    Console.WriteLine($"Same logical configuration polled 3 times. Controller writes: {sync.AppliedZones.Count}");
    bool symptomObserved = sync.AppliedZones.Count == 3;
    Console.WriteLine(symptomObserved ? "REPRODUCED: unexpected repeated writes" : "NOT REPRODUCED: check starter state");
    return symptomObserved ? 0 : 1;
}

static int Verify()
{
    var cases = new (string Name, int Expected, Action<CalibrationSync> Arrange)[]
    {
        ("equal values in fresh snapshots", 1, sync =>
        {
            sync.Observe(new("zone-A", 7, new[] { 18, 35, 62 }));
            sync.Observe(new("zone-A", 7, new[] { 18, 35, 62 }));
        }),
        ("revision change", 2, sync =>
        {
            sync.Observe(new("zone-A", 7, new[] { 18, 35 }));
            sync.Observe(new("zone-A", 8, new[] { 18, 35 }));
        }),
        ("threshold content change", 2, sync =>
        {
            sync.Observe(new("zone-A", 7, new[] { 18, 35 }));
            sync.Observe(new("zone-A", 7, new[] { 18, 36 }));
        }),
        ("different zones", 2, sync =>
        {
            sync.Observe(new("zone-A", 7, new[] { 18, 35 }));
            sync.Observe(new("zone-B", 7, new[] { 18, 35 }));
        }),
        ("same instance unchanged", 1, sync =>
        {
            var profile = new CalibrationProfile("zone-A", 7, new[] { 18, 35 });
            sync.Observe(profile);
            sync.Observe(profile);
        }),
        ("caller mutates reused input", 2, sync =>
        {
            var thresholds = new[] { 18, 35 };
            var profile = new CalibrationProfile("zone-A", 7, thresholds);
            sync.Observe(profile);
            thresholds[0] = 19;
            sync.Observe(profile);
        }),
        ("cached state immune to caller mutation", 1, sync =>
        {
            var thresholds = new[] { 18, 35 };
            sync.Observe(new("zone-A", 7, thresholds));
            thresholds[0] = 19;
            sync.Observe(new("zone-A", 7, new[] { 18, 35 }));
        }),
        ("threshold ordering is significant", 2, sync =>
        {
            sync.Observe(new("zone-A", 7, new[] { 18, 35 }));
            sync.Observe(new("zone-A", 7, new[] { 35, 18 }));
        }),
        ("threshold count changes", 2, sync =>
        {
            sync.Observe(new("zone-A", 7, new[] { 18, 35 }));
            sync.Observe(new("zone-A", 7, new[] { 18, 35, 62 }));
        })
    };

    int failures = 0;
    foreach (var test in cases)
    {
        var sync = new CalibrationSync();
        test.Arrange(sync);
        int actual = sync.AppliedZones.Count;
        bool passed = actual == test.Expected;
        Console.WriteLine($"{(passed ? "PASS" : "FAIL")} {test.Name}: expected={test.Expected}, actual={actual}");
        if (!passed) failures++;
    }

    Console.WriteLine($"Verification: {cases.Length - failures}/{cases.Length} passed.");
    return failures == 0 ? 0 : 1;
}
