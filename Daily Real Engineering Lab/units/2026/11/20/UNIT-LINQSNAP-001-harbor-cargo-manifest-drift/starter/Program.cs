using System.Globalization;

var cargo = new List<Cargo>
{
    new("CT-101", 12m, "Ready"),
    new("CT-102", 18m, "Ready"),
    new("CT-103", 4m, "Held")
};

var manifest = ManifestService.Confirm(cargo);

// A later operational event occurs before the external export is serialized.
cargo[1].WeightTonnes = 24m;
cargo[1].Status = "Reweighed";

var exported = manifest.OrderBy(x => x.ContainerId).ToArray();
var actual = exported.Sum(x => x.WeightTonnes);
Console.WriteLine("EXPECTED=30");
Console.WriteLine("ACTUAL=" + actual.ToString(CultureInfo.InvariantCulture));
Console.WriteLine("DETAIL=" + string.Join(",", exported.Select(x => x.ContainerId + ":" + x.WeightTonnes.ToString(CultureInfo.InvariantCulture))));

if (args.Contains("--verify", StringComparer.OrdinalIgnoreCase))
{
    bool pass = exported.Length == 2 &&
        exported[0] == new ManifestRow("CT-101", 12m) &&
        exported[1] == new ManifestRow("CT-102", 18m) &&
        actual == 30m;
    Console.WriteLine(pass ? "VERIFY PASS" : "VERIFY FAIL");
    return pass ? 0 : 1;
}

if (args.Contains("--reproduce", StringComparer.OrdinalIgnoreCase))
{
    bool symptom = actual != 30m;
    Console.WriteLine(symptom ? "REPRODUCED" : "NOT REPRODUCED");
    return symptom ? 0 : 1;
}
return 0;

public sealed class Cargo(string containerId, decimal weightTonnes, string status)
{
    public string ContainerId { get; } = containerId;
    public decimal WeightTonnes { get; set; } = weightTonnes;
    public string Status { get; set; } = status;
}

public sealed record ManifestRow(string ContainerId, decimal WeightTonnes);

public static class ManifestService
{
    public static IEnumerable<ManifestRow> Confirm(IEnumerable<Cargo> cargo)
    {
        return cargo.Where(x => x.Status == "Ready")
            .Select(x => new ManifestRow(x.ContainerId, x.WeightTonnes));
    }
}
