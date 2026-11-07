using System;
using System.Collections.Generic;

var registry = new Dictionary<LoanKey, string>();
var key = new LoanKey("MUS-042", 2026);
registry.Add(key, "APPROVED");
Console.WriteLine($"Before correction: {registry.TryGetValue(new LoanKey("MUS-042", 2026), out var before)} / {before}");
key.Year = 2027; // Registrar corrected the loan year after import.
var found = registry.TryGetValue(key, out var status);
Console.WriteLine($"After correction: found={found}, status={status ?? "<missing>"}, count={registry.Count}");
if (!found || status != "APPROVED") Environment.ExitCode = 1;

public sealed class LoanKey : IEquatable<LoanKey>
{
    public string ArtifactId { get; }
    public int Year { get; set; }
    public LoanKey(string artifactId, int year) { ArtifactId = artifactId; Year = year; }
    public bool Equals(LoanKey? other) => other is not null && ArtifactId == other.ArtifactId && Year == other.Year;
    public override bool Equals(object? obj) => obj is LoanKey other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(ArtifactId, Year);
}
