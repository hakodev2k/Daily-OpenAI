using System;
using System.Collections.Generic;
using System.Linq;

record Backup(string Id, int CreatedDay, int RetainUntilDay, string KeyVersion, bool ChecksumValid);
static class RecoveryPolicy
{
    public static HashSet<string> RetainedKeys(IEnumerable<Backup> backups, int today, IEnumerable<string> allKeys)
    {
        // Rotation maintenance currently keeps only the active version.
        var required = backups.Where(b => b.RetainUntilDay >= today).Select(b => b.KeyVersion).ToHashSet(StringComparer.Ordinal);
        return allKeys.Where(required.Contains).ToHashSet(StringComparer.Ordinal);
    }
}
static class Program
{
    static readonly Backup[] Backups = [
        new("archive-jan", 1, 91, "k1", true),
        new("archive-feb", 31, 121, "k2", true),
        new("archive-mar", 61, 151, "k3", true)
    ];
    static int Main(string[] args)
    {
        int day = 70;
        var available = RecoveryPolicy.RetainedKeys(Backups, day, ["k1", "k2", "k3"]);
        var live = Backups.Where(b => b.RetainUntilDay >= day).ToArray();
        var failed = live.Where(b => b.ChecksumValid && !available.Contains(b.KeyVersion)).ToArray();
        Console.WriteLine($"day={day} retained={string.Join(",", available.Order())} live={live.Length} unrestorable={failed.Length}");
        foreach (var b in failed) Console.WriteLine($"restore-failed backup={b.Id} key={b.KeyVersion} checksum={b.ChecksumValid}");
        if (args.Contains("--reproduce")) return failed.Length == 2 ? 0 : 2;
        if (args.Contains("--verify")) return failed.Length == 0 && available.SetEquals(new[] {"k1", "k2", "k3"}) ? 0 : 3;
        return 0;
    }
}
