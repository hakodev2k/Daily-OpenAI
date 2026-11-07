static void Put(string root, string name, string value)
    => File.WriteAllText(Path.Combine(root, name), value);

static bool Matches(IReadOnlyDictionary<string, string> actual,
    params (string Name, string Value)[] expected)
    => actual.Count == expected.Length &&
       expected.All(x => actual.TryGetValue(x.Name, out var v) &&
                         string.Equals(v, x.Value, StringComparison.Ordinal));

static string Describe(IReadOnlyDictionary<string, string> snapshot)
    => string.Join(",", snapshot.OrderBy(x => x.Key, StringComparer.Ordinal)
        .Select(x => x.Key + "=" + x.Value));

var root = Path.Combine(Path.GetTempPath(), "certificate-index-lab-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);
try
{
    Put(root, "CAL-A.cert", "valid:v1");
    Put(root, "CAL-B.cert", "valid:v1");
    Put(root, "CAL-C.cert", "review:v1");
    Put(root, "scratch.tmp", "ignore-me");

    var indexer = new CertificateIndexer(root);
    indexer.OnNotification("CAL-A.cert");
    indexer.OnNotification("CAL-B.cert");
    indexer.OnNotification("CAL-C.cert");
    var initial = indexer.Snapshot();

    Put(root, "CAL-A.cert", "suspended:v2");
    Put(root, "CAL-B.cert", "valid:v2");
    indexer.OnNotification("CAL-B.cert");
    File.Delete(Path.Combine(root, "CAL-C.cert"));
    Put(root, "CAL-D.cert", "valid:v1");
    indexer.MaintenanceTick();
    var phase1 = indexer.Snapshot();
    indexer.MaintenanceTick();
    var repeat = indexer.Snapshot();

    File.Delete(Path.Combine(root, "CAL-B.cert"));
    indexer.OnNotification("CAL-B.cert");
    Put(root, "CAL-D.cert", "expired:v2");
    Put(root, "CAL-E.cert", "valid:v1");
    indexer.MaintenanceTick();
    var phase2 = indexer.Snapshot();

    Console.WriteLine("initialIndex=" + Describe(initial));
    Console.WriteLine("phase1Index=" + Describe(phase1));
    Console.WriteLine("phase1Repeat=" + Describe(repeat));
    Console.WriteLine("phase2Index=" + Describe(phase2));
    Console.WriteLine("notificationsReceived=" + indexer.NotificationsReceived);
    Console.WriteLine("maintenanceTicks=" + indexer.MaintenanceTicks);

    bool initialOk = Matches(initial, ("CAL-A.cert", "valid:v1"),
        ("CAL-B.cert", "valid:v1"), ("CAL-C.cert", "review:v1"));
    bool phase1Ok = Matches(phase1, ("CAL-A.cert", "suspended:v2"),
        ("CAL-B.cert", "valid:v2"), ("CAL-D.cert", "valid:v1"));
    bool repeatOk = Matches(repeat, ("CAL-A.cert", "suspended:v2"),
        ("CAL-B.cert", "valid:v2"), ("CAL-D.cert", "valid:v1"));
    bool phase2Ok = Matches(phase2, ("CAL-A.cert", "suspended:v2"),
        ("CAL-D.cert", "expired:v2"), ("CAL-E.cert", "valid:v1"));

    if (initialOk && phase1Ok && repeatOk && phase2Ok &&
        indexer.NotificationsReceived == 5 && indexer.MaintenanceTicks == 3)
    {
        Console.WriteLine("LAB_RESULT=PASS");
        return 0;
    }

    bool original1 = Matches(phase1, ("CAL-A.cert", "valid:v1"),
        ("CAL-B.cert", "valid:v2"), ("CAL-C.cert", "review:v1"));
    bool original2 = Matches(phase2, ("CAL-A.cert", "valid:v1"),
        ("CAL-C.cert", "review:v1"));
    if (initialOk && original1 && original2 &&
        indexer.NotificationsReceived == 5 && indexer.MaintenanceTicks == 3)
    {
        Console.WriteLine("LAB_RESULT=EXPECTED_FAILURE");
        return 3;
    }

    Console.WriteLine("LAB_RESULT=UNEXPECTED");
    return 4;
}
finally
{
    Directory.Delete(root, recursive: true);
}
