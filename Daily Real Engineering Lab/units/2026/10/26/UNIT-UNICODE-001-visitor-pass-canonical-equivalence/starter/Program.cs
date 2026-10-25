using VisitorPassLab;

var scans = new[]
{
    new BadgeScan("E-001", "PASS-Caf\u00E9-27", "mobile-ios"),
    new BadgeScan("E-002", "PASS-Cafe\u0301-27", "lobby-kiosk"),
    new BadgeScan("E-003", "PASS-CAF\u00C9-27", "mobile-ios"),
    new BadgeScan("E-004", "PASS-Caf\u00E9-27", "side-gate")
};

var accepted = BadgeImporter.Import(scans);
var actualIds = accepted.Select(scan => scan.EventId).ToArray();

Console.WriteLine($"Input events: {scans.Length}");
Console.WriteLine($"Accepted: {accepted.Count}");
Console.WriteLine($"Accepted IDs: {string.Join(", ", actualIds)}");

var mode = args.Length == 0 ? "--run" : args[0];
if (mode == "--run") return 0;

if (mode == "--reproduce")
{
    // Chỉ xác nhận starter có triệu chứng dự kiến, không phải xác nhận fix.
    bool reproduced = actualIds.SequenceEqual(
        new[] { "E-001", "E-002", "E-003" }, StringComparer.Ordinal);
    Console.WriteLine(reproduced
        ? "REPRODUCED: logically equivalent passes were admitted twice."
        : "NOT REPRODUCED: starter symptom differs from baseline.");
    return reproduced ? 0 : 1;
}

if (mode == "--verify")
{
    // Contract: giữ first-seen, gộp định danh tương đương, giữ mã khác case.
    bool idsCorrect = actualIds.SequenceEqual(
        new[] { "E-001", "E-003" }, StringComparer.Ordinal);
    bool originalPayloadPreserved = accepted.Count == 2
        && accepted[0].PassCode == scans[0].PassCode
        && accepted[1].PassCode == scans[2].PassCode;
    bool inputUnchanged = scans[1].PassCode == "PASS-Cafe\u0301-27";
    bool passed = idsCorrect && originalPayloadPreserved && inputUnchanged;
    Console.WriteLine(passed
        ? "PASS: identity, first-seen, case distinction and payload invariants hold."
        : "FAIL: identity contract or regression invariant violated.");
    return passed ? 0 : 1;
}

Console.Error.WriteLine("Usage: --run | --reproduce | --verify");
return 2;
