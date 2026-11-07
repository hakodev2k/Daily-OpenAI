using System;
using System.Collections.Generic;
using System.Linq;

string mode = args.Length == 0 ? "show" : args[0].ToLowerInvariant();
var replicaA = new FlagEvaluator(0u);
var replicaB = new FlagEvaluator(1u);
const string feature = "water-chart-v2";
const string tenant = "north-district";
string[] users = Enumerable.Range(1, 200).Select(i => $"resident-{i:D4}").ToArray();
int mismatches = users.Count(id =>
    replicaA.Bucket(feature, tenant, id) != replicaB.Bucket(feature, tenant, id));

if (mode == "show")
{
    foreach (string id in users.Take(8))
        Console.WriteLine($"{id}: A={replicaA.Bucket(feature, tenant, id)} B={replicaB.Bucket(feature, tenant, id)}");
    Console.WriteLine($"Bucket mismatches: {mismatches}/{users.Length}");
    return 0;
}
if (mode == "reproduce")
{
    Console.WriteLine($"Same-identity replica mismatches: {mismatches}/{users.Length}");
    if (mismatches == 0) { Console.Error.WriteLine("NOT REPRODUCED"); return 1; }
    Console.WriteLine("REPRODUCED: cohort is not replica-independent.");
    return 0;
}
if (mode != "verify") { Console.Error.WriteLine("Usage: show | reproduce | verify"); return 2; }

var failures = new List<string>();
foreach (string id in users)
{
    int a = replicaA.Bucket(feature, tenant, id);
    int b = replicaB.Bucket(feature, tenant, id);
    if (a != b) failures.Add($"Replica mismatch: {id}: {a}/{b}");
    if (a is < 0 or >= 10_000) failures.Add($"Out-of-range bucket: {id}");
    if (a != replicaA.Bucket(feature, tenant, id)) failures.Add($"Unstable repeat: {id}");
    if (replicaA.IsEnabled(feature, tenant, id, 2_000) &&
        !replicaA.IsEnabled(feature, tenant, id, 4_000))
        failures.Add($"Non-monotonic rollout: {id}");
    if (replicaA.IsEnabled(feature, tenant, id, 0)) failures.Add($"0% enabled: {id}");
    if (!replicaA.IsEnabled(feature, tenant, id, 10_000)) failures.Add($"100% disabled: {id}");
}
int enabled50 = Enumerable.Range(1, 1000)
    .Count(i => replicaA.IsEnabled(feature, tenant, $"population-{i:D4}", 5_000));
if (enabled50 is < 400 or > 600)
    failures.Add($"Unexpected 50% distribution: {enabled50}/1000");
int separated = users.Count(id => replicaA.Bucket(feature, tenant, id) !=
    replicaA.Bucket(feature, "south-district", id));
if (separated < 180) failures.Add($"Tenant separation too weak: {separated}/200");
foreach (int invalid in new[] { -1, 10_001 })
{
    try
    {
        replicaA.IsEnabled(feature, tenant, "resident-0001", invalid);
        failures.Add($"Invalid threshold accepted: {invalid}");
    }
    catch (ArgumentOutOfRangeException) { }
}
if (failures.Count != 0)
{
    foreach (string f in failures.Take(10)) Console.Error.WriteLine(f);
    Console.Error.WriteLine($"VERIFY FAILED: {failures.Count} assertions.");
    return 1;
}
Console.WriteLine($"VERIFY PASSED: stable cohorts, boundaries, distribution. Enabled50={enabled50}/1000");
return 0;
