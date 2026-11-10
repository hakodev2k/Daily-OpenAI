using System;
using System.Collections.Generic;

var inventory = new HashSet<SeedLot>();
var lot = new SeedLot("LOT-42", "A-01");
inventory.Add(lot);
bool before = inventory.Contains(lot);
lot.MoveTo("COLD-07");
bool after = inventory.Contains(lot);
bool removed = inventory.Remove(lot);
Console.WriteLine($"count-before-removal={inventory.Count + (removed ? 1 : 0)}");
Console.WriteLine($"contains-before={before}");
Console.WriteLine($"contains-after={after}");
Console.WriteLine($"remove-after={removed}");

if (args.Length > 0 && args[0] == "verify")
{
    // Verify the learner-editable implementation, never a separate solution.
    var other = new SeedLot("LOT-43", "COLD-07");
    bool distinct = !lot.Equals(other);
    if (!before || !after || !removed || !distinct || inventory.Count != 0)
    {
        Console.Error.WriteLine("FAIL: inventory lookup/removal invariant");
        Environment.ExitCode = 1;
    }
    else Console.WriteLine("PASS: inventory lookup/removal invariant");
}

public sealed class SeedLot : IEquatable<SeedLot>
{
    public string LotId { get; }
    public string Shelf { get; private set; }
    public SeedLot(string lotId, string shelf) { LotId = lotId; Shelf = shelf; }
    public void MoveTo(string shelf) => Shelf = shelf;
    public bool Equals(SeedLot? other) => other is not null &&
        LotId == other.LotId && Shelf == other.Shelf;
    public override bool Equals(object? obj) => obj is SeedLot other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(LotId, Shelf);
}
