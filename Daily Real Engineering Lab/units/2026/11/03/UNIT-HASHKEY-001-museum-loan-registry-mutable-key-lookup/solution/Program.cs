using System;
using System.Collections.Generic;

var registry = new Dictionary<LoanKey, string>();
var oldKey = new LoanKey("MUS-042", 2026);
registry.Add(oldKey, "APPROVED");
Console.WriteLine($"Before correction: {registry.TryGetValue(new LoanKey("MUS-042", 2026), out var before)} / {before}");
var newKey = oldKey with { Year = 2027 };
if (!registry.Remove(oldKey, out var status)) throw new InvalidOperationException("Old key not found");
registry.Add(newKey, status);
var found = registry.TryGetValue(newKey, out var updated);
Console.WriteLine($"After correction: found={found}, status={updated ?? "<missing>"}, count={registry.Count}");
if (!found || updated != "APPROVED" || registry.ContainsKey(oldKey) || registry.Count != 1) Environment.ExitCode = 1;
public sealed record LoanKey(string ArtifactId, int Year);
