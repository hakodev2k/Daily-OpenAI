using TraceLab;

var mode = args.Length == 0 ? "run" : args[0];
if (mode is not ("run" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Use: run | reproduce | verify");
    return 2;
}

var scenarios = new[]
{
    (Name: "single-tile", Tiles: new[] { "TILE-01" }),
    (Name: "parallel-two", Tiles: new[] { "TILE-11", "TILE-12" }),
    (Name: "parallel-six", Tiles: new[] { "TILE-21", "TILE-22", "TILE-23", "TILE-24", "TILE-25", "TILE-26" })
};

var failures = 0;
var parallelFailures = 0;
var singleFailures = 0;
foreach (var scenario in scenarios)
{
    var rows = await TileAuditPipeline.ProcessAsync(scenario.Tiles);
    if (rows.Length != scenario.Tiles.Length)
    {
        Console.Error.WriteLine("Missing audit rows: " + scenario.Name);
        return 2;
    }

    var mismatches = rows.Count(row => row.TileId != row.AuditTileId);
    failures += mismatches;
    if (scenario.Tiles.Length > 1) parallelFailures += mismatches;
    else singleFailures += mismatches;

    Console.WriteLine(scenario.Name + ": rows=" + rows.Length + ", mismatches=" + mismatches);
    foreach (var row in rows)
        Console.WriteLine("  expected=" + row.TileId + ", audit=" + row.AuditTileId);
}

if (mode == "reproduce")
{
    var reproduced = parallelFailures > 0 && singleFailures == 0;
    Console.WriteLine(reproduced ? "REPRODUCED: parallel attribution differs." : "NOT REPRODUCED");
    return reproduced ? 0 : 1;
}

if (mode == "verify")
{
    Console.WriteLine(failures == 0 ? "PASS: all audit correlations match." : "FAIL: audit correlation mismatch.");
    return failures == 0 ? 0 : 1;
}

return 0;
