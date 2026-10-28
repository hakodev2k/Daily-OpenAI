using ManifestAudit;

var mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
var evidence = RunScenario();

Console.WriteLine($"first_snapshot_count_after_next_scan={evidence.FirstCountAfterNextScan} (expected 1)");
Console.WriteLine($"second_snapshot_count_after_reader_attempt={evidence.SecondCountAfterReaderAttempt} (expected 2)");
Console.WriteLine($"reader_could_mutate_snapshot={evidence.ReaderCouldMutateSnapshot} (expected False)");
Console.WriteLine($"active_manifest_unchanged_by_reader={evidence.ActiveManifestUnaffected} (expected True)");
Console.WriteLine($"timestamps_preserved={evidence.TimestampsPreserved} (expected True)");
Console.WriteLine($"other_flight_isolated={evidence.OtherFlightIsolated} (expected True)");
Console.WriteLine($"verification_passed={evidence.IsCorrect}");

return mode switch
{
    "demo" => 0,
    "reproduce" => evidence.FirstCountAfterNextScan != 1
        && evidence.ReaderCouldMutateSnapshot
        && !evidence.ActiveManifestUnaffected ? 0 : 1,
    "verify" => evidence.IsCorrect ? 0 : 1,
    _ => InvalidMode(mode)
};

static int InvalidMode(string mode)
{
    Console.Error.WriteLine($"Unknown mode '{mode}'. Use demo, reproduce, or verify.");
    return 2;
}

static Evidence RunScenario()
{
    var baseTime = new DateTimeOffset(2026, 10, 28, 0, 0, 0, TimeSpan.Zero);
    var service = new ManifestService();

    service.OpenFlight("VN-318", baseTime);
    service.OpenFlight("VN-319", baseTime);
    service.ScanBag("VN-318", "BAG-01", baseTime.AddMinutes(1));
    var first = service.Capture("VN-318", baseTime.AddMinutes(2));

    service.ScanBag("VN-318", "BAG-02", baseTime.AddMinutes(3));
    var second = service.Capture("VN-318", baseTime.AddMinutes(4));
    var firstCountAfterNextScan = first.BagTags.Count;

    var readerCouldMutateSnapshot = false;
    if (first.BagTags is IList<string> writableView)
    {
        try
        {
            writableView.Add("UNAUTHORIZED-BAG");
            readerCouldMutateSnapshot = true;
        }
        catch (NotSupportedException)
        {
            // A read-only collection refuses the attempted modification.
        }
    }

    var secondCountAfterReaderAttempt = second.BagTags.Count;
    var active = service.Capture("VN-318", baseTime.AddMinutes(5));
    var activeManifestUnaffected = active.BagTags.SequenceEqual(new[] { "BAG-01", "BAG-02" });

    service.ScanBag("VN-319", "BAG-B1", baseTime.AddMinutes(6));
    var otherFlight = service.Capture("VN-319", baseTime.AddMinutes(7));
    var otherFlightIsolated = otherFlight.BagTags.SequenceEqual(new[] { "BAG-B1" })
        && active.FlightId == "VN-318";

    var timestampsPreserved = first.CapturedAtUtc == baseTime.AddMinutes(2)
        && second.CapturedAtUtc == baseTime.AddMinutes(4);

    var isCorrect = firstCountAfterNextScan == 1
        && first.BagTags.SequenceEqual(new[] { "BAG-01" })
        && secondCountAfterReaderAttempt == 2
        && second.BagTags.SequenceEqual(new[] { "BAG-01", "BAG-02" })
        && !readerCouldMutateSnapshot
        && activeManifestUnaffected
        && otherFlightIsolated
        && timestampsPreserved;

    return new Evidence(firstCountAfterNextScan, secondCountAfterReaderAttempt,
        readerCouldMutateSnapshot, activeManifestUnaffected, timestampsPreserved,
        otherFlightIsolated, isCorrect);
}

internal sealed record Evidence(
    int FirstCountAfterNextScan,
    int SecondCountAfterReaderAttempt,
    bool ReaderCouldMutateSnapshot,
    bool ActiveManifestUnaffected,
    bool TimestampsPreserved,
    bool OtherFlightIsolated,
    bool IsCorrect);
