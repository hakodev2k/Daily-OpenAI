using FoodBankDispatch;

var mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();
var day = new DateOnly(2026, 11, 2);

try
{
    if (mode is "demo" or "reproduce")
    {
        var batch = new List<DonationLot>
        {
            Lot("FB-101", -1),
            Lot("FB-102", -2),
            Lot("FB-103", 2),
            Lot("FB-104", 0)
        };
        Console.WriteLine("Input:  " + string.Join(", ", batch.Select(x => x.LotId)));
        var result = DonationDispatch.Prepare(batch, day);
        Console.WriteLine("Output: " + string.Join(", ", result.Select(x => x.LotId)));
        var invalid = result.Where(x => x.IsQuarantined || x.BestBefore < day).Select(x => x.LotId).ToArray();
        Console.WriteLine("Ineligible in output: " + string.Join(", ", invalid));

        if (mode == "reproduce")
        {
            if (invalid.Length == 0)
                throw new InvalidOperationException("Expected starter symptom not observed; starter may already be fixed.");
            Console.WriteLine("REPRODUCED: batch eligibility contract violated.");
        }
    }
    else if (mode == "verify")
    {
        AssertBatch("adjacent expired lots",
            new[] { Lot("A", -1), Lot("B", -2), Lot("C", 2) }, "C");
        AssertBatch("all ineligible",
            new[] { Lot("A", -1), Lot("B", -1, true), Lot("C", 2, true) });
        AssertBatch("alternating eligible and expired",
            new[] { Lot("A", -1), Lot("B", 2), Lot("C", -2), Lot("D", 0) }, "B", "D");
        AssertBatch("quarantine independent of expiry",
            new[] { Lot("A", 3, true), Lot("B", 4, true), Lot("C", 1) }, "C");
        AssertBatch("exact best-before date is eligible",
            new[] { Lot("A", 0), Lot("B", 1) }, "A", "B");
        AssertBatch("empty batch", Array.Empty<DonationLot>());
        var reusable = new List<DonationLot> { Lot("A", -1), Lot("B", -2), Lot("C", 2), Lot("D", 0) };
        DonationDispatch.Prepare(reusable, day);
        var second = DonationDispatch.Prepare(reusable, day);
        if (!second.Select(x => x.LotId).SequenceEqual(new[] { "C", "D" }))
            throw new InvalidOperationException("Second invocation changed the eligible set.");
        Console.WriteLine("VERIFIED: 7 behavior contracts passed on learner-editable starter.");
    }
    else
    {
        throw new ArgumentException("Mode must be demo, reproduce, or verify.");
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine("FAILED: " + ex.Message);
    Environment.ExitCode = 1;
}

static DonationLot Lot(string id, int daysFromPickup, bool quarantine = false)
    => new(id, new DateOnly(2026, 11, 2).AddDays(daysFromPickup), quarantine);

static void AssertBatch(string name, DonationLot[] input, params string[] expected)
{
    var batch = input.ToList();
    var output = DonationDispatch.Prepare(batch, new DateOnly(2026, 11, 2));
    var ids = output.Select(x => x.LotId).ToArray();
    if (!ReferenceEquals(batch, output))
        throw new InvalidOperationException(name + ": expected in-place batch ownership.");
    if (!ids.SequenceEqual(expected))
        throw new InvalidOperationException(name + ": expected [" + string.Join(",", expected)
            + "], got [" + string.Join(",", ids) + "].");
    if (output.Any(x => x.IsQuarantined || x.BestBefore < new DateOnly(2026, 11, 2)))
        throw new InvalidOperationException(name + ": ineligible lot survived.");
    Console.WriteLine("PASS: " + name);
}
